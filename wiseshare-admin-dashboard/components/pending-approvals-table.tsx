// components/PendingApprovalsTable.tsx
"use client"
import { useState, useEffect } from "react"
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"
import {
  Dialog,
  DialogContent,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Check, SearchIcon } from "lucide-react"
import {
  getAllProperties,
  getPendingSells,
  getUserById,
  approveSellRequest,
} from "@/lib/api"

interface PendingSell {
  investmentId: string
  userId: string
  propertyId: string
  sharesToSell: number
  requestedAt: string
}

interface Property {
  id: string
  name: string
}

interface User {
  firstName: string
  lastName: string
  email: string
}

export function PendingApprovalsTable() {
  const [pendingSells, setPendingSells] = useState<PendingSell[]>([])
  const [filteredSells, setFilteredSells] = useState<PendingSell[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [isApproveDialogOpen, setIsApproveDialogOpen] = useState(false)
  const [selectedSell, setSelectedSell] = useState<PendingSell | null>(null)
  const [processing, setProcessing] = useState(false)
  const [searchTerm, setSearchTerm] = useState("")
  const [userCache, setUserCache] = useState<Record<string, User>>({})
  const [propertyCache, setPropertyCache] = useState<Record<string, Property>>({})

  useEffect(() => {
    fetchPendingSells()
  }, [])

  useEffect(() => {
    if (!searchTerm.trim()) {
      setFilteredSells(pendingSells)
      return
    }
    const term = searchTerm.toLowerCase()
    setFilteredSells(
      pendingSells.filter((s) => {
        const u = userCache[s.userId]
        const p = propertyCache[s.propertyId]
        return (
          (u && `${u.firstName} ${u.lastName}`.toLowerCase().includes(term)) ||
          (u && u.email.toLowerCase().includes(term)) ||
          (p && p.name.toLowerCase().includes(term)) ||
          s.userId.toLowerCase().includes(term) ||
          s.propertyId.toLowerCase().includes(term)
        )
      })
    )
  }, [searchTerm, pendingSells, userCache, propertyCache])

  async function fetchPendingSells() {
    setLoading(true)
    const props = (await getAllProperties()) as Property[]
    setPropertyCache(
      props.reduce((acc, p) => {
        acc[p.id] = p
        return acc
      }, {} as Record<string, Property>)
    )
    const pending = (await getPendingSells()) as PendingSell[]
    setPendingSells(pending)
    setFilteredSells(pending)
    const uids: string[] = Array.from(new Set(pending.map((x) => x.userId)))
    const users = await Promise.all(
      uids.map(async (id) => ({ id, user: (await getUserById(id)) as User }))
    )
    setUserCache(
      users.reduce((acc, { id, user }) => {
        acc[id] = user
        return acc
      }, {} as Record<string, User>)
    )
    setLoading(false)
  }

  function handleApprove(s: PendingSell) {
    setSelectedSell(s)
    setIsApproveDialogOpen(true)
  }

  async function confirmApprove() {
    if (!selectedSell) return
    setProcessing(true)
    await approveSellRequest(selectedSell.userId, selectedSell.propertyId)
    setIsApproveDialogOpen(false)
    fetchPendingSells()
    setProcessing(false)
  }

  return (
    <div className="space-y-4">
      <div className="relative">
        <SearchIcon className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
        <Input
          type="search"
          placeholder="Search pending approvals..."
          className="w-full pl-8"
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
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
              <TableHead>Request Date</TableHead>
              <TableHead className="text-right">Actions</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {loading ? (
              <TableRow>
                <TableCell colSpan={6} className="text-center py-10">
                  Loading…
                </TableCell>
              </TableRow>
            ) : error ? (
              <TableRow>
                <TableCell colSpan={6} className="text-center py-10 text-destructive">
                  {error}
                </TableCell>
              </TableRow>
            ) : filteredSells.length === 0 ? (
              <TableRow>
                <TableCell colSpan={6} className="text-center py-10">
                  No pending approvals.
                </TableCell>
              </TableRow>
            ) : (
              filteredSells.map((s) => (
                <TableRow key={`${s.investmentId}-${s.userId}`}>
                  <TableCell className="font-mono text-xs">{s.investmentId}</TableCell>
                  <TableCell className="font-medium">
                    {userCache[s.userId]
                      ? `${userCache[s.userId].firstName} ${userCache[s.userId].lastName}`
                      : s.userId}
                  </TableCell>
                  <TableCell>{propertyCache[s.propertyId]?.name || s.propertyId}</TableCell>
                  <TableCell>{s.sharesToSell}</TableCell>
                  <TableCell>{new Date(s.requestedAt).toLocaleDateString()}</TableCell>
                  <TableCell className="text-right">
                    <Button onClick={() => handleApprove(s)} variant="outline" size="sm">
                      <Check className="h-4 w-4 text-green-500" />
                    </Button>
                  </TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
      </div>
      <Dialog open={isApproveDialogOpen} onOpenChange={() => setIsApproveDialogOpen(false)}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Approve Sell?</DialogTitle>
          </DialogHeader>
          <DialogFooter>
            <Button variant="outline" onClick={() => setIsApproveDialogOpen(false)} disabled={processing}>
              Cancel
            </Button>
            <Button onClick={confirmApprove} disabled={processing}>
              Confirm
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  )
}
