"use client"

import { useState, useEffect } from "react"
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Badge } from "@/components/ui/badge"
import { getInvestmentsByProperty, getAllProperties, getUserById } from "@/lib/api"
import { format } from "date-fns"

export function InvestmentDetails({ investmentId }: { investmentId: string }) {
  const [investment, setInvestment] = useState(null)
  const [user, setUser] = useState(null)
  const [property, setProperty] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)

  useEffect(() => {
    async function fetchInvestmentData() {
      try {
        setLoading(true)

        // Get all properties first
        const properties = await getAllProperties()

        // Find the investment by searching through all properties' investments
        let foundInvestment = null
        let foundProperty = null

        for (const prop of properties) {
          try {
            const investments = await getInvestmentsByProperty(prop.id)
            const found = investments.find((inv) => inv.id === investmentId)
            if (found) {
              foundInvestment = found
              foundProperty = prop
              break
            }
          } catch (err) {
            console.error(`Failed to fetch investments for property ${prop.id}:`, err)
          }
        }

        if (foundInvestment && foundProperty) {
          setInvestment(foundInvestment)
          setProperty(foundProperty)

          // Fetch user details
          try {
            const userData = await getUserById(foundInvestment.userId)
            setUser(userData)
          } catch (err) {
            console.error("Failed to fetch user data:", err)
          }

          setError(null)
        } else {
          setError("Investment not found")
        }
      } catch (err) {
        console.error("Failed to fetch investment data:", err)
        setError(err.message)
      } finally {
        setLoading(false)
      }
    }

    fetchInvestmentData()
  }, [investmentId])

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary mx-auto"></div>
          <p className="mt-4 text-muted-foreground">Loading investment data...</p>
        </div>
      </div>
    )
  }

  if (error) {
    return (
      <Card>
        <CardHeader>
          <CardTitle className="text-destructive">Error</CardTitle>
          <CardDescription>Failed to load investment data</CardDescription>
        </CardHeader>
        <CardContent>
          <p>{error}</p>
        </CardContent>
        <CardFooter>
          <Button variant="outline" onClick={() => window.history.back()}>
            Go Back
          </Button>
        </CardFooter>
      </Card>
    )
  }

  if (!investment) {
    return (
      <Card>
        <CardHeader>
          <CardTitle>Investment Not Found</CardTitle>
          <CardDescription>The requested investment could not be found</CardDescription>
        </CardHeader>
        <CardContent>
          <p>The investment with ID {investmentId} does not exist or has been deleted.</p>
        </CardContent>
        <CardFooter>
          <Button variant="outline" onClick={() => window.history.back()}>
            Go Back
          </Button>
        </CardFooter>
      </Card>
    )
  }

  return (
    <div className="space-y-6">
      <Card>
        <CardHeader>
          <CardTitle className="text-2xl">Investment Details</CardTitle>
          <CardDescription className="mt-1">Investment ID: {investment.id}</CardDescription>
        </CardHeader>
        <CardContent>
          <div className="grid gap-6 md:grid-cols-2">
            <div className="space-y-4">
              <div>
                <h3 className="text-sm font-medium text-muted-foreground">User</h3>
                <p>{user ? `${user.firstName} ${user.lastName}` : "Unknown User"}</p>
                <p className="text-xs text-muted-foreground">User ID: {investment.userId}</p>
              </div>
              <div>
                <h3 className="text-sm font-medium text-muted-foreground">Property</h3>
                <p>{property ? property.name : "Unknown Property"}</p>
                <p className="text-xs text-muted-foreground">Property ID: {investment.propertyId}</p>
              </div>
              <div>
                <h3 className="text-sm font-medium text-muted-foreground">Shares Purchased</h3>
                <p>{investment.numOfSharesPurchased}</p>
              </div>
              <div>
                <h3 className="text-sm font-medium text-muted-foreground">avg Share Price</h3>
                <p>${investment.sharePrice?.toLocaleString() || "0"}</p>
              </div>
            </div>
            <div className="space-y-4">
              <div>
                <h3 className="text-sm font-medium text-muted-foreground">Investment Amount</h3>
                <p>${investment.investmentAmount?.toLocaleString() || "0"}</p>
              </div>
              <div>
                <h3 className="text-sm font-medium text-muted-foreground">Investment Date</h3>
                <p>{investment.createdAt ? format(new Date(investment.createdAt), "PPP 'at' p") : "Unknown"}</p>
              </div>
              <div>
                <h3 className="text-sm font-medium text-muted-foreground">Status</h3>
                <Badge variant={investment.sellRequest ? "outline" : "default"}>
                  {investment.sellRequest ? "Sell Requested" : "Active"}
                </Badge>
              </div>
              {investment.sellRequest && investment.sellRequestDate && (
                <div>
                  <h3 className="text-sm font-medium text-muted-foreground">Sell Request Date</h3>
                  <p>{format(new Date(investment.sellRequestDate), "PPP 'at' p")}</p>
                </div>
              )}
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  )
}
