// src/lib/api.ts
const API_BASE_URL = "https://wiseshareapi.aliarthur.com"

async function handleResponse(response: Response) {
  if (!response.ok) {
    // Start with HTTP status line
    let errorMsg = `${response.status} ${response.statusText}`.trim()

    // Read raw response body
    const text = await response.text()
    if (text) {
      try {
        const body = JSON.parse(text)

        // 1) Top‑level array of errors?
        if (Array.isArray(body.errors)) {
          errorMsg = body.errors.map((msg: any) => ` ${msg}`).join("\n")
        }
        // 2) Field→array validation errors?
        else if (body.errors && typeof body.errors === "object") {
          errorMsg = Object.entries(body.errors)
            .flatMap(([field, errs]) =>
              (errs as string[]).map(msg => `• ${field}: ${msg}`)
            )
            .join("\n")
        }
        // 3) Single message or title?
        else if (body.message) {
          errorMsg = body.message
        } else if (body.title) {
          errorMsg = body.title
        }
        // 4) Fallback to raw text
        else {
          errorMsg = text
        }
      } catch {
        // Non‑JSON body, use raw text
        errorMsg = text
      }
    }

    // **THIS LINE MUST THROW** so your `try/catch` in components catches it:
    throw new Error(errorMsg)
  }

  // Success branch: parse JSON if any, else null
  const content = await response.text()
  if (!content) return null
  try {
    return JSON.parse(content)
  } catch {
    return null
  }
}


// ——— Users ———

export async function getAllUsers() {
  const response = await fetch(`${API_BASE_URL}/api/users/search/All`)
  return handleResponse(response)
}

export async function getUserByEmail(email: string) {
  const response = await fetch(
    `${API_BASE_URL}/api/users/search/email/${encodeURIComponent(email)}`
  )
  return handleResponse(response)
}

export async function getUserByPhone(phone: string) {
  const response = await fetch(
    `${API_BASE_URL}/api/users/search/phone/${encodeURIComponent(phone)}`
  )
  return handleResponse(response)
}

export async function getUserById(id: string) {
  const response = await fetch(`${API_BASE_URL}/api/users/search/id/${id}`)
  return handleResponse(response)
}

export async function updateUser(
  id: string,
  userData: {
    email: string
    phone: string
    password: string
    role: string
    securityQuestion: string
    securityAnswer: string
  }
): Promise<void> {
  const response = await fetch(
    `${API_BASE_URL}/api/admin/users/${id}`,
    {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(userData),
    }
  )

  if (!response.ok) {
    // try to parse a JSON error payload, or fall back to raw text/status
    let errorText: string
    try {
      const errBody = await response.json()
      // if it has a `message` property, use that
      if (errBody?.message) {
        errorText = errBody.message
      }
      // if it has a nested validation `errors` object, flatten it
      else if (errBody?.errors && typeof errBody.errors === "object") {
        errorText = Object.entries(errBody.errors)
          .flatMap(([field, msgs]) =>
            (msgs as string[]).map(m => `${field}: ${m}`)
          )
          .join("; ")
      } else {
        // otherwise stringify the entire body
        errorText = JSON.stringify(errBody)
      }
    } catch {
      // if not valid JSON, fall back to raw text or status
      errorText = (await response.text()) || `${response.status} ${response.statusText}`
    }

    console.error(`updateUser(${id}) failed:`, errorText)
    throw new Error(errorText)
  }

  // no response body expected on success; simply return
}

export async function deactivateUser(id: string) {
  const response = await fetch(
    `${API_BASE_URL}/api/admin/users/${id}/deactivate`,
    { method: "POST" }
  )
  return handleResponse(response)
}

export async function reactivateUser(id: string) {
  const response = await fetch(
    `${API_BASE_URL}/api/admin/users/${id}/reactivate`,
    { method: "POST" }
  )
  return handleResponse(response)
}
export async function toggleUserStatus(id: string, isActive: boolean) {
  if (isActive) {
    return deactivateUser(id)
  } else {
    return reactivateUser(id)
  }
}

// ------Authentication ------

// POST /auth/register
export async function registerUser(userData: {
  firstName: string
  lastName: string
  email: string
  phone: string
  password: string
  securityQuestion: string
  securityAnswer: string
}) {
  const response = await fetch(`${API_BASE_URL}/auth/register`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(userData),
  })
  return handleResponse(response)
}


// ——— Properties ———

export async function getAllProperties() {
  const response = await fetch(`${API_BASE_URL}/api/property/all`)
  return handleResponse(response)
}

export async function getPropertyById(id: string) {
  const response = await fetch(
    `${API_BASE_URL}/api/property/search/id/${id}`
  )
  return handleResponse(response)
}

export async function createProperty(propertyData: any) {
  const response = await fetch(`${API_BASE_URL}/api/property/create`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(propertyData),
  })
  return handleResponse(response)
}

export async function updateProperty(
  id: string,
  propertyData: any
) {
  const response = await fetch(
    `${API_BASE_URL}/api/admin/properties/${id}`,
    {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(propertyData),
    }
  )
  return handleResponse(response)
}

export async function deleteProperty(id: string) {
  const response = await fetch(
    `${API_BASE_URL}/api/admin/properties/${id}`,
    { method: "DELETE" }
  )
  return handleResponse(response)
}

export async function uploadPropertyImages(
  id: string,
  formData: FormData
) {
  const response = await fetch(
    `${API_BASE_URL}/api/property/${id}/upload-images`,
    { method: "POST", body: formData }
  )
  return handleResponse(response)
}

export async function deletePropertyImages(
  id: string,
  fileNames: string[]
) {
  const query = fileNames
    .map((n) => `fileNames=${encodeURIComponent(n)}`)
    .join("&")
  const response = await fetch(
    `${API_BASE_URL}/api/property/${id}/images?${query}`,
    { method: "DELETE" }
  )
  return handleResponse(response)
}

// ——— Property investment status helpers ———

export async function togglePropertyInvestment(id: string, isEnabled: boolean) {
  const url = isEnabled
    ? `${API_BASE_URL}/api/admin/${id}/disable-investments`
    : `${API_BASE_URL}/api/admin/${id}/enable-investments`

  const response = await fetch(url, { method: "POST" })
  return handleResponse(response)
}

// ——— Portfolios ———

export async function getUserPortfolio(userId: string) {
  const response = await fetch(
    `${API_BASE_URL}/api/portfolio/user/${userId}`
  )
  return handleResponse(response)
}

// ——— Investments ———

export async function getInvestmentsByUser(userId: string): Promise<any[]> {
  try {
    const response = await fetch(
      `${API_BASE_URL}/api/investment/user/${userId}`
    )
    return (await handleResponse(response)) as any[]
  } catch (err: any) {
    if (err.message.includes("No investments found")) {
      return []
    }
    throw err
  }
}

export async function getInvestmentsByProperty(propertyId: string): Promise<any[]> {
  try {
    const response = await fetch(
      `${API_BASE_URL}/api/investment/property/${propertyId}`
    )
    return (await handleResponse(response)) as any[]
  } catch (err: any) {
    if (err.message.includes("No investments found")) {
      return []
    }
    throw err
  }
}

export async function requestSellShares(
  userId: string,
  propertyId: string,
  numberOfShares: number
) {
  const response = await fetch(
    `${API_BASE_URL}/api/investment/request-sell`,
    {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        UserId: userId,
        PropertyId: propertyId,
        NumberOfSharesToSell: numberOfShares,
      }),
    }
  )
  return handleResponse(response)
}

export async function approveSellRequest(
  userId: string,
  propertyId: string
) {
  const response = await fetch(
    `${API_BASE_URL}/api/investment/approve-sell`,
    {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ UserId: userId, PropertyId: propertyId }),
    }
  )
  return handleResponse(response)
}

export async function getPendingSells() {
  const response = await fetch(
    `${API_BASE_URL}/api/investment/pending`
  )
  return handleResponse(response)
}

// ——— Payments ———

export async function getAllPayments() {
  const response = await fetch(`${API_BASE_URL}/api/admin/payments`)
  return handleResponse(response)
}

export async function getFilteredPayments(filter: any) {
  const response = await fetch(
    `${API_BASE_URL}/api/admin/payments/search`,
    {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(filter),
    }
  )
  return handleResponse(response)
}

export async function getUserPayments(userId: string) {
  const response = await fetch(
    `${API_BASE_URL}/api/payment/search/user/${userId}`
  )
  return handleResponse(response)
}

export async function getPaymentById(id: string) {
  const response = await fetch(
    `${API_BASE_URL}/api/payment/search/id/${id}`
  )
  return handleResponse(response)
}

export async function processRefund(
  userId: string,
  amount: number,
  paymentIntentId: string
) {
  const response = await fetch(
    `${API_BASE_URL}/api/payment/refund`,
    {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ UserId: userId, Amount: amount, PaymentIntentId: paymentIntentId }),
    }
  )
  return handleResponse(response)
}

export async function processWithdrawal(
  userId: string,
  amount: number
) {
  const response = await fetch(
    `${API_BASE_URL}/api/payment/withdraw-stripe`,
    {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ UserId: userId, Amount: amount }),
    }
  )
  return handleResponse(response)
}

// ——— Wallet ———
export interface WalletBalanceDto {
  balance: number
  updatedAt: string
}

export async function getWalletBalance(
  userId: string
): Promise<WalletBalanceDto> {
  const response = await fetch(
    `${API_BASE_URL}/api/wallet/user/${encodeURIComponent(userId)}`
  )
  // handleResponse will parse { userId, balance, updatedAt }
  return handleResponse(response) as Promise<WalletBalanceDto>
}
