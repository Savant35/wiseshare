"use client"

import { useState, useEffect } from "react"
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "@/components/ui/card"
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import { Button } from "@/components/ui/button"
import { Badge } from "@/components/ui/badge"
import { getUserById, getUserPayments, getInvestmentsByUser, toggleUserStatus } from "@/lib/api"
import { getWalletBalance } from "@/lib/api"
import Link from "next/link"
import { ArrowLeft, UserCheck, UserX } from "lucide-react"
import { format } from "date-fns"
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription, DialogFooter, } from "@/components/ui/dialog"


export function UserProfile({ userId }: { userId: string }) {
  const [user, setUser] = useState(null)
  const [payments, setPayments] = useState([])
  const [investments, setInvestments] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)
  const [popupOpen, setPopupOpen] = useState(false)
  const [popupMessage, setPopupMessage] = useState("")
  const [wallet, setWallet] = useState<{ balance: number; updatedAt: string } | null>(null)



  // Add toggle user status function
  const handleToggleUserStatus = async () => {
    const action = user.isActive ? "deactivate" : "activate"
    //if (!confirm(`Are you sure you want to ${action} this user account?`)) return
    try {
      setLoading(true)
      // pass the _current_ flag, so true → /deactivate, false → /reactivate
      await toggleUserStatus(userId, user.isActive)

      const updatedUser = await getUserById(userId)
      setUser(updatedUser)

      const newState = updatedUser.isActive ? "activated" : "deactivated"
      setPopupMessage(`User account ${newState} successfully`)
      setPopupOpen(true)
    } catch (err: any) {
      console.error("Failed to toggle user status:", err)
      setPopupMessage(`Failed to toggle user status: ${err.message}`)
      setPopupOpen(true)
    } finally {
      setLoading(false)
    }
  }


  useEffect(() => {
    async function fetchUserData() {
      try {
        setLoading(true)
        const [userData, paymentsData, investmentsData, walletData] = await Promise.all([
          getUserById(userId),
          getUserPayments(userId).catch(() => []),
          getInvestmentsByUser(userId).catch(() => []),
          getWalletBalance(userId),
        ])
        setUser(userData)
        setPayments(paymentsData)
        setInvestments(investmentsData)
        setWallet(walletData)
        setError(null)
      } catch (err: any) {
        console.error("Failed to fetch user data:", err)
        setError(err.message)
      } finally {
        setLoading(false)
      }
    }

    fetchUserData()
  }, [userId])


  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary mx-auto"></div>
          <p className="mt-4 text-muted-foreground">Loading user data...</p>
        </div>
      </div>
    )
  }

  if (error) {
    return (
      <Card>
        <CardHeader>
          <CardTitle className="text-destructive">Error</CardTitle>
          <CardDescription>Failed to load user data</CardDescription>
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

  if (!user) {
    return (
      <Card>
        <CardHeader>
          <CardTitle>User Not Found</CardTitle>
          <CardDescription>The requested user could not be found</CardDescription>
        </CardHeader>
        <CardContent>
          <p>The user with ID {userId} does not exist or has been deleted.</p>
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
        <CardHeader className="flex flex-row items-start justify-between">
          <div>
            <CardTitle className="text-2xl">
              {user.firstName} {user.lastName}
            </CardTitle>
            <CardDescription className="mt-1">User ID: {user.id}</CardDescription>
          </div>
          <div className="flex items-center space-x-2">
            <Link href="/dashboard/users">
              <Button variant="outline" size="sm">
                <ArrowLeft className="mr-2 h-4 w-4" />
                Back to Users
              </Button>
            </Link>
          </div>
        </CardHeader>
        <CardContent>
          <div className="grid gap-6 md:grid-cols-2">
            <div className="space-y-4">
              <div>
                <h3 className="text-sm font-medium text-muted-foreground">Email</h3>
                <p>{user.email}</p>
              </div>
              <div>
                <h3 className="text-sm font-medium text-muted-foreground">Phone</h3>
                <p>{user.phone || "Not provided"}</p>
              </div>
              <div>
                <h3 className="text-sm font-medium text-muted-foreground">Wallet Balance</h3>
                <p>
                  {wallet ? `$${wallet.balance.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 })}` : "Loading…"}</p>
              </div>

              <div>
                <h3 className="text-sm font-medium text-muted-foreground">Role</h3>
                <Badge variant={user.role === "Admin" ? "default" : "secondary"}>
                  {user.role === "Investor" ? "User" : user.role}
                </Badge>
              </div>
            </div>
            <div className="space-y-4">
              <div>
                <h3 className="text-sm font-medium text-muted-foreground">Created</h3>
                <p>{user.createdAt ? format(new Date(user.createdAt), "PPP 'at' p") : "Unknown"}</p>
              </div>
              <div>
                <h3 className="text-sm font-medium text-muted-foreground">Last Updated</h3>
                <p>{user.updatedAt ? format(new Date(user.updatedAt), "PPP 'at' p") : "Never"}</p>
              </div>
              <div>
                <h3 className="text-sm font-medium text-muted-foreground">Status</h3>
                <div className="flex items-center gap-2 mt-1">
                  <Badge variant={user.isActive ? "default" : "destructive"}>
                    {user.isActive ? "Active" : "Inactive"}
                  </Badge>
                  <Button
                    variant={user.isActive ? "destructive" : "default"}
                    size="sm"
                    onClick={handleToggleUserStatus}
                    className="ml-2"
                    disabled={loading}
                  >
                    {user.isActive ? (
                      <>
                        <UserX className="mr-2 h-4 w-4" />
                        Deactivate Account
                      </>
                    ) : (
                      <>
                        <UserCheck className="mr-2 h-4 w-4" />
                        Activate Account
                      </>
                    )}
                  </Button>
                </div>
              </div>
            </div>
          </div>
        </CardContent>
      </Card>

      <Tabs defaultValue="investments" className="w-full">
        <TabsList className="grid w-full grid-cols-2">
          <TabsTrigger value="investments">Investments ({investments.length})</TabsTrigger>
          <TabsTrigger value="payments">Payment History ({payments.length})</TabsTrigger>
        </TabsList>
        <TabsContent value="investments" className="mt-4">
          <Card>
            <CardHeader>
              <CardTitle>Investment History</CardTitle>
              <CardDescription>All investments made by this user</CardDescription>
            </CardHeader>
            <CardContent>
              {investments.length === 0 ? (
                <p className="text-center py-4 text-muted-foreground">No investments found for this user.</p>
              ) : (
                <div className="space-y-4">
                  {investments.map((investment) => (
                    <div key={investment.id} className="flex items-center justify-between border-b pb-4">
                      <div>
                        <p className="font-medium">Property ID: {investment.propertyId}</p>
                        <p className="text-sm text-muted-foreground">
                          {investment.numOfSharesPurchased} shares at ${investment.sharePrice} per share
                        </p>
                        <p className="text-xs text-muted-foreground">
                          {new Date(investment.createdAt).toLocaleDateString()}
                        </p>
                      </div>
                      <div className="text-right">
                        <p className="font-medium">${investment.investmentAmount.toLocaleString()}</p>
                        <Badge variant={investment.sellRequest ? "outline" : "default"}>
                          {investment.sellRequest ? "Sell Requested" : "Active"}
                        </Badge>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </CardContent>
          </Card>
        </TabsContent>
        <TabsContent value="payments" className="mt-4">
          <Card>
            <CardHeader>
              <CardTitle>Payment History</CardTitle>
              <CardDescription>All payments made by this user</CardDescription>
            </CardHeader>
            <CardContent>
              {payments.length === 0 ? (
                <p className="text-center py-4 text-muted-foreground">No payment history found for this user.</p>
              ) : (
                <div className="space-y-4">
                  {payments.map((payment) => (
                    <div key={payment.id} className="flex items-center justify-between border-b pb-4">
                      <div>
                        <p className="font-medium">{payment.type}</p>
                        <p className="text-sm text-muted-foreground">ID: {payment.id}</p>
                        <p className="text-xs text-muted-foreground">
                          {new Date(payment.createdAt).toLocaleDateString()}
                        </p>
                      </div>
                      <div className="text-right">
                        <p className="font-medium">${payment.amount.toLocaleString()}</p>
                        <Badge
                          variant={
                            payment.status === "Completed"
                              ? "default"
                              : payment.status === "Pending"
                                ? "outline"
                                : "destructive"
                          }
                        >
                          {payment.status}
                        </Badge>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </CardContent>
          </Card>
        </TabsContent>
      </Tabs>
      <Dialog open={popupOpen} onOpenChange={setPopupOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>{popupMessage.startsWith("Failed") ? "Error" : "Success"}</DialogTitle>
          </DialogHeader>
          <DialogDescription>{popupMessage}</DialogDescription>
          <DialogFooter>
            <Button onClick={() => setPopupOpen(false)}>OK</Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  )
}
