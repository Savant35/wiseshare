"use client"

import { useState, useEffect } from "react"
import { Table,TableBody, TableCell,
  TableHead, TableHeader,TableRow,} from "@/components/ui/table"
import {Dialog, DialogContent, DialogDescription, DialogFooter,DialogHeader,DialogTitle,} from "@/components/ui/dialog"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"                        
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem,DropdownMenuLabel,DropdownMenuSeparator, DropdownMenuTrigger,} from "@/components/ui/dropdown-menu"
import { Badge } from "@/components/ui/badge"
import { MoreHorizontal, RefreshCw, SearchIcon } from "lucide-react"
import { getAllPayments, processRefund, getUserById } from "@/lib/api"

interface Payment {
  id: string
  userId: string
  amount: number
  type: string
  status: string
  createdAt: string
  stripePaymentIntentId?: string
}

export function PaymentTable() {
  const [payments, setPayments] = useState<Payment[]>([])              // ← typed state
  const [filteredPayments, setFilteredPayments] = useState<Payment[]>([])
  const [loading, setLoading] = useState<boolean>(true)
  const [error, setError] = useState<string | null>(null)
  const [isRefundDialogOpen, setIsRefundDialogOpen] = useState<boolean>(false)
  const [selectedPayment, setSelectedPayment] = useState<Payment | null>(null)
  const [refundAmount, setRefundAmount] = useState<string>("")
  const [processing, setProcessing] = useState<boolean>(false)
  const [searchTerm, setSearchTerm] = useState<string>("")
  const [userCache, setUserCache] = useState<Record<string, any>>({})
  const [errorDialogOpen, setErrorDialogOpen] = useState<boolean>(false)
  const [errorDialogMessage, setErrorDialogMessage] = useState<string>("")

  useEffect(() => {
    fetchPayments()
  }, [])

  useEffect(() => {
    if (!searchTerm.trim()) {
      setFilteredPayments(payments)
      return
    }
    const term = searchTerm.toLowerCase()
    setFilteredPayments(
      payments.filter((payment) => {
        const user = userCache[payment.userId]
        const fullName = user
          ? `${user.firstName} ${user.lastName}`.toLowerCase()
          : ""
        const email = user?.email.toLowerCase() ?? ""
        return (
          payment.id.toLowerCase().includes(term) ||
          payment.type.toLowerCase().includes(term) ||
          payment.status.toLowerCase().includes(term) ||
          fullName.includes(term) ||
          email.includes(term)
        )
      })
    )
  }, [searchTerm, payments, userCache])

  async function fetchPayments() {
    setLoading(true)
    try {
      const data = (await getAllPayments()) as Payment[]
      // fetch user info
      const ids = Array.from(new Set(data.map((p) => p.userId)))
      const details = await Promise.all(
        ids.map(async (uid) => {
          try {
            return { uid, user: await getUserById(uid) }
          } catch {
            return { uid, user: null }
          }
        })
      )
      const map: Record<string, any> = {}
      details.forEach(({ uid, user }) => (map[uid] = user))
      setUserCache(map)
      setPayments(data)
      setFilteredPayments(data)
      setError(null)
    } catch (err: any) {
      setError(err.message)
    } finally {
      setLoading(false)
    }
  }

  const handleRefund = (payment: Payment) => {
    setSelectedPayment(payment)
    setRefundAmount(payment.amount.toString())
    setIsRefundDialogOpen(true)
  }

  const confirmRefund = async () => {
    if (!selectedPayment) return
    setProcessing(true)
    try {
      await processRefund(
        selectedPayment.userId,
        parseFloat(refundAmount),
        selectedPayment.stripePaymentIntentId || ""
      )
      setIsRefundDialogOpen(false)
      fetchPayments()
    } catch (err: any) {
      setErrorDialogMessage(`Failed to process refund: ${err.message}`)
      setErrorDialogOpen(true)
    } finally {
      setProcessing(false)
    }
  }

  return (
    <div className="space-y-4">
      {/* SEARCH */}
      <div className="relative">
        <SearchIcon className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
        <Input
          type="search"
          placeholder="Search payments..."
          className="w-full pl-8"
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
        />
      </div>

      {/* TABLE */}
      <div className="rounded-md border">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>User</TableHead>
              <TableHead>Amount</TableHead>
              <TableHead>Type</TableHead>
              <TableHead>Status</TableHead>
              <TableHead>Date</TableHead>
              <TableHead className="text-right">Actions</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {loading ? (
              <TableRow>
                <TableCell colSpan={6} className="text-center py-10">
                  Loading payments...
                </TableCell>
              </TableRow>
            ) : error ? (
              <TableRow>
                <TableCell colSpan={6} className="text-center py-10 text-destructive">
                  {error}
                </TableCell>
              </TableRow>
            ) : filteredPayments.length === 0 ? (
              <TableRow>
                <TableCell colSpan={6} className="text-center py-10">
                  {searchTerm
                    ? "No payments found matching your search."
                    : "No payments found."}
                </TableCell>
              </TableRow>
            ) : (
              filteredPayments.map((payment) => {
                const user = userCache[payment.userId]
                return (
                  <TableRow key={payment.id}>
                    <TableCell className="font-medium">
                      {user ? (
                        <>
                          <div>
                            {user.firstName} {user.lastName}
                          </div>
                          <div className="text-xs text-muted-foreground">{user.email}</div>
                        </>
                      ) : (
                        `User ID: ${payment.userId}`
                      )}
                    </TableCell>
                    <TableCell>${payment.amount.toLocaleString()}</TableCell>
                    <TableCell>{payment.type}</TableCell>
                    <TableCell>
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
                    </TableCell>
                    <TableCell>
                      {new Date(payment.createdAt).toLocaleDateString()}
                    </TableCell>
                    <TableCell className="text-right">
                      <DropdownMenu>
                        <DropdownMenuTrigger asChild>
                          <Button variant="ghost" className="h-8 w-8 p-0">
                            <span className="sr-only">Open menu</span>
                            <MoreHorizontal className="h-4 w-4" />
                          </Button>
                        </DropdownMenuTrigger>
                        <DropdownMenuContent align="end">
                          <DropdownMenuLabel>Actions</DropdownMenuLabel>
                          {payment.type === "Deposit" &&
                            payment.status === "Completed" && (
                              <DropdownMenuItem onClick={() => handleRefund(payment)}>
                                <RefreshCw className="mr-2 h-4 w-4" />
                                Process Refund
                              </DropdownMenuItem>
                            )}
                          <DropdownMenuSeparator />
                        </DropdownMenuContent>
                      </DropdownMenu>
                    </TableCell>
                  </TableRow>
                )
              })
            )}
          </TableBody>
        </Table>
      </div>

      {/* REFUND DIALOG */}
      <Dialog open={isRefundDialogOpen} onOpenChange={setIsRefundDialogOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Process Refund</DialogTitle>
            <DialogDescription>
              Enter the amount to refund to the user.
            </DialogDescription>
          </DialogHeader>
          {selectedPayment && (
            <div className="grid gap-4 py-4">
              <div className="grid grid-cols-4 items-center gap-4">
                <Label htmlFor="userName" className="text-right">
                  User
                </Label>
                <Input
                  id="userName"
                  value={
                    userCache[selectedPayment.userId]
                      ? `${userCache[selectedPayment.userId].firstName} ${userCache[selectedPayment.userId].lastName}`
                      : `User ID: ${selectedPayment.userId}`
                  }
                  className="col-span-3"
                  disabled
                />
              </div>
              <div className="grid grid-cols-4 items-center gap-4">
                <Label htmlFor="refundAmount" className="text-right">
                  Refund Amount
                </Label>
                <div className="col-span-3 flex items-center">
                  <span className="mr-2">$</span>
                  <Input
                    id="refundAmount"
                    value={refundAmount}
                    onChange={(e) => setRefundAmount(e.target.value)}
                    className="flex-1"
                  />
                </div>
              </div>
            </div>
          )}
          <DialogFooter>
            <Button
              variant="outline"
              onClick={() => setIsRefundDialogOpen(false)}
              disabled={processing}
            >
              Cancel
            </Button>
            <Button onClick={confirmRefund} disabled={processing}>
              {processing ? "Processing..." : "Process Refund"}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* ERROR DIALOG */}
      <Dialog open={errorDialogOpen} onOpenChange={setErrorDialogOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Error</DialogTitle>
            <DialogDescription>{errorDialogMessage}</DialogDescription>
          </DialogHeader>
          <DialogFooter>
            <Button variant="outline" onClick={() => setErrorDialogOpen(false)}>
              OK
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  )
}
