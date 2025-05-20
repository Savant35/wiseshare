"use client"

import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table"
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu"
import { useState, useEffect } from "react"
import { MoreHorizontal, Check, Eye, SearchIcon } from "lucide-react"
import { getInvestmentsByProperty, getAllProperties, approveSellRequest, getUserById } from "@/lib/api"
import Link from "next/link"

export function InvestmentTable() {
  const [investments, setInvestments] = useState([])
  const [filteredInvestments, setFilteredInvestments] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)
  const [isApproveDialogOpen, setIsApproveDialogOpen] = useState(false)
  const [selectedInvestment, setSelectedInvestment] = useState(null)
  const [processing, setProcessing] = useState(false)
  const [searchTerm, setSearchTerm] = useState("")
  const [userCache, setUserCache] = useState({})
  const [propertyCache, setPropertyCache] = useState({})

  useEffect(() => {
    fetchInvestments()
  }, [])

  useEffect(() => {
    if (searchTerm.trim() === "") {
      setFilteredInvestments(investments)
      return
    }

    const filtered = investments.filter((investment) => {
      const user = userCache[investment.userId]
      const property = propertyCache[investment.propertyId]
      const userFullName = user ? `${user.firstName} ${user.lastName}`.toLowerCase() : ""
      const userEmail = user ? user.email.toLowerCase() : ""
      const propertyName = property ? property.name.toLowerCase() : ""

      return (
        userFullName.includes(searchTerm.toLowerCase()) ||
        userEmail.includes(searchTerm.toLowerCase()) ||
        propertyName.includes(searchTerm.toLowerCase()) ||
        investment.propertyId.toLowerCase().includes(searchTerm.toLowerCase()) ||
        investment.userId.toLowerCase().includes(searchTerm.toLowerCase()) ||
        investment.id.toLowerCase().includes(searchTerm.toLowerCase())
      )
    })

    setFilteredInvestments(filtered)
  }, [searchTerm, investments, userCache, propertyCache])

  async function fetchInvestments() {
    try {
      setLoading(true)
      // Since there's no endpoint to get all investments, we'll get them by property
      const properties = await getAllProperties()
      setPropertyCache(
        properties.reduce((acc, property) => {
          acc[property.id] = property
          return acc
        }, {}),
      )

      let allInvestments = []
      for (const property of properties) {
        try {
          const propertyInvestments = await getInvestmentsByProperty(property.id)
          allInvestments = [...allInvestments, ...propertyInvestments]
        } catch (err) {
          console.error(`Failed to fetch investments for property ${property.id}:`, err)
        }
      }

      // Fetch user details for each investment
      const userIds = [...new Set(allInvestments.map((i) => i.userId))]
      const userDetailsPromises = userIds.map(async (userId) => {
        try {
          const user = await getUserById(userId)
          return { userId, user }
        } catch (err) {
          console.error(`Failed to fetch user ${userId}:`, err)
          return { userId, user: null }
        }
      })

      const userDetails = await Promise.all(userDetailsPromises)
      const userMap = userDetails.reduce((acc, { userId, user }) => {
        acc[userId] = user
        return acc
      }, {})

      setUserCache(userMap)
      setInvestments(allInvestments)
      setFilteredInvestments(allInvestments)
      setError(null)
    } catch (err) {
      console.error("Failed to fetch investments:", err)
      setError(err.message)
    } finally {
      setLoading(false)
    }
  }

  const handleApprove = (investment) => {
    setSelectedInvestment(investment)
    setIsApproveDialogOpen(true)
  }

  const confirmApprove = async () => {
    try {
      setProcessing(true)
      await approveSellRequest(selectedInvestment.userId, selectedInvestment.propertyId)
      setIsApproveDialogOpen(false)
      // Refresh the investments list
      fetchInvestments()
    } catch (error) {
      console.error("Failed to approve sell request:", error)
      alert(`Failed to approve sell request: ${error.message}`)
    } finally {
      setProcessing(false)
    }
  }

  const handleSearch = (e) => {
    setSearchTerm(e.target.value)
  }

  return (
    <div className="space-y-4">
      <div className="relative">
        <SearchIcon className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
        <Input
          type="search"
          placeholder="Search investments..."
          className="w-full pl-8"
          value={searchTerm}
          onChange={handleSearch}
        />
      </div>

      <div className="rounded-md border">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Investment ID</TableHead>
              <TableHead>User</TableHead>
              <TableHead>Property</TableHead>
              <TableHead>Shares</TableHead>
              <TableHead>Investment Amount</TableHead>
              <TableHead>Date</TableHead>
              <TableHead className="text-right">Actions</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {loading ? (
              <TableRow>
                <TableCell colSpan={7} className="text-center py-10">
                  Loading investments...
                </TableCell>
              </TableRow>
            ) : error ? (
              <TableRow>
                <TableCell colSpan={7} className="text-center py-10 text-destructive">
                  Error: {error}
                </TableCell>
              </TableRow>
            ) : filteredInvestments.length === 0 ? (
              <TableRow>
                <TableCell colSpan={7} className="text-center py-10">
                  {searchTerm ? "No investments found matching your search." : "No investments found."}
                </TableCell>
              </TableRow>
            ) : (
              filteredInvestments.map((investment) => {
                const user = userCache[investment.userId]
                const property = propertyCache[investment.propertyId]
                return (
                  <TableRow key={investment.id}>
                    <TableCell className="font-mono text-xs">{investment.id}</TableCell>
                    <TableCell className="font-medium">
                      {user ? (
                        <>
                          <div>
                            {user.firstName} {user.lastName}
                          </div>
                          <div className="text-xs text-muted-foreground">
                            {user.email} (ID: {investment.userId})
                          </div>
                        </>
                      ) : (
                        `User ID: ${investment.userId}`
                      )}
                    </TableCell>
                    <TableCell>
                      {property ? (
                        <>
                          <div>{property.name}</div>
                          <div className="text-xs text-muted-foreground">ID: {investment.propertyId}</div>
                        </>
                      ) : (
                        `Property ID: ${investment.propertyId}`
                      )}
                    </TableCell>
                    <TableCell>{investment.numOfSharesPurchased}</TableCell>
                    <TableCell>${investment.investmentAmount?.toLocaleString() || "0"}</TableCell>
                    <TableCell>{new Date(investment.createdAt).toLocaleDateString()}</TableCell>
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
                          <DropdownMenuItem asChild>
                            <Link href={`/dashboard/investments/${investment.id}`}>
                              <Eye className="mr-2 h-4 w-4" />
                              View details
                            </Link>
                          </DropdownMenuItem>
                          {investment.sellRequest && (
                            <DropdownMenuItem onClick={() => handleApprove(investment)}>
                              <Check className="mr-2 h-4 w-4" />
                              Approve sell request
                            </DropdownMenuItem>
                          )}
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

      <Dialog open={isApproveDialogOpen} onOpenChange={setIsApproveDialogOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Approve Sell Request</DialogTitle>
            <DialogDescription>
              Are you sure you want to approve this sell request? This action cannot be undone.
            </DialogDescription>
          </DialogHeader>
          {selectedInvestment && (
            <div className="py-4">
              <div className="grid gap-4">
                <div className="grid grid-cols-4 items-center gap-4">
                  <Label htmlFor="investmentId" className="text-right">
                    Investment ID
                  </Label>
                  <Input id="investmentId" value={selectedInvestment.id} className="col-span-3" disabled />
                </div>
                <div className="grid grid-cols-4 items-center gap-4">
                  <Label htmlFor="userId" className="text-right">
                    User ID
                  </Label>
                  <Input id="userId" value={selectedInvestment.userId} className="col-span-3" disabled />
                </div>
                <div className="grid grid-cols-4 items-center gap-4">
                  <Label htmlFor="userName" className="text-right">
                    User
                  </Label>
                  <Input
                    id="userName"
                    value={
                      userCache[selectedInvestment.userId]
                        ? `${userCache[selectedInvestment.userId].firstName} ${userCache[selectedInvestment.userId].lastName}`
                        : "Unknown User"
                    }
                    className="col-span-3"
                    disabled
                  />
                </div>
                <div className="grid grid-cols-4 items-center gap-4">
                  <Label htmlFor="userEmail" className="text-right">
                    Email
                  </Label>
                  <Input
                    id="userEmail"
                    value={
                      userCache[selectedInvestment.userId]
                        ? userCache[selectedInvestment.userId].email
                        : "Email not available"
                    }
                    className="col-span-3"
                    disabled
                  />
                </div>
                <div className="grid grid-cols-4 items-center gap-4">
                  <Label htmlFor="propertyId" className="text-right">
                    Property ID
                  </Label>
                  <Input id="propertyId" value={selectedInvestment.propertyId} className="col-span-3" disabled />
                </div>
                <div className="grid grid-cols-4 items-center gap-4">
                  <Label htmlFor="property" className="text-right">
                    Property
                  </Label>
                  <Input
                    id="property"
                    value={
                      propertyCache[selectedInvestment.propertyId]
                        ? propertyCache[selectedInvestment.propertyId].name
                        : "Unknown Property"
                    }
                    className="col-span-3"
                    disabled
                  />
                </div>
                <div className="grid grid-cols-4 items-center gap-4">
                  <Label htmlFor="shares" className="text-right">
                    Shares
                  </Label>
                  <Input id="shares" value={selectedInvestment.numOfSharesPurchased} className="col-span-3" disabled />
                </div>
                <div className="grid grid-cols-4 items-center gap-4">
                  <Label htmlFor="amount" className="text-right">
                    Amount
                  </Label>
                  <Input
                    id="amount"
                    value={`$${selectedInvestment.investmentAmount?.toLocaleString() || "0"}`}
                    className="col-span-3"
                    disabled
                  />
                </div>
              </div>
            </div>
          )}
          <DialogFooter>
            <Button variant="outline" onClick={() => setIsApproveDialogOpen(false)} disabled={processing}>
              Cancel
            </Button>
            <Button onClick={confirmApprove} disabled={processing}>
              {processing ? "Processing..." : "Approve"}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  )
}
