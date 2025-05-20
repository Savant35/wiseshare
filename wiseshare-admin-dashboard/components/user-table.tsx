"use client"

import { useState, useEffect } from "react"
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table"
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader,DialogTitle,} from "@/components/ui/dialog"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import {DropdownMenu, DropdownMenuContent, DropdownMenuItem,DropdownMenuLabel, DropdownMenuSeparator,DropdownMenuTrigger,} from "@/components/ui/dropdown-menu"
import { updateUser } from "@/lib/api"
import { Badge } from "@/components/ui/badge"

import { MoreHorizontal, Pencil, Eye, SearchIcon, UserCheck, UserX } from "lucide-react"
import { getAllUsers, getUserByEmail, getUserByPhone, toggleUserStatus } from "@/lib/api"
import Link from "next/link"
import { API_BASE_URL } from "@/config"

export function UserTable() {
  const [users, setUsers] = useState([])
  const [filteredUsers, setFilteredUsers] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)
  const [editUser, setEditUser] = useState(null)
  const [isEditDialogOpen, setIsEditDialogOpen] = useState(false)
  const [processing, setProcessing] = useState(false)
  const [searchTerm, setSearchTerm] = useState("")
  const [searchType, setSearchType] = useState("all") // all, email, phone

  useEffect(() => {
    fetchUsers()
  }, [])

  useEffect(() => {
    if (searchTerm.trim() === "") {
      setFilteredUsers(users)
      return
    }

    const delaySearch = setTimeout(async () => {
      try {
        if (searchType === "email") {
          const result = await getUserByEmail(searchTerm)
          setFilteredUsers(Array.isArray(result) ? result : [result])
        } else if (searchType === "phone") {
          const result = await getUserByPhone(searchTerm)
          setFilteredUsers(Array.isArray(result) ? result : [result])
        } else {
          // Simple client-side filtering for "all" option
          const filtered = users.filter(
            (user) =>
              user.email.toLowerCase().includes(searchTerm.toLowerCase()) ||
              user.phone?.toLowerCase().includes(searchTerm.toLowerCase()) ||
              `${user.firstName} ${user.lastName}`.toLowerCase().includes(searchTerm.toLowerCase()) ||
              user.id.toLowerCase().includes(searchTerm.toLowerCase()),
          )
          setFilteredUsers(filtered)
        }
      } catch (err) {
        console.error("Search error:", err)
        // If search fails, show empty results
        setFilteredUsers([])
      }
    }, 500)

    return () => clearTimeout(delaySearch)
  }, [searchTerm, searchType, users])

  async function fetchUsers() {
    try {
      setLoading(true)
      const data = await getAllUsers()
      setUsers(data)
      setFilteredUsers(data)
      setError(null)
    } catch (err) {
      console.error("Failed to fetch users:", err)
      setError(err.message)
    } finally {
      setLoading(false)
    }
  }

  const handleEditUser = (user) => {
    setEditUser(user)
    setIsEditDialogOpen(true)
  }

  // Update the handleUpdateUser function to handle non-JSON responses
  const handleUpdateUser = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault()
    try {
      setProcessing(true)
      const formData = new FormData(e.currentTarget)
      const updatedUser = {
        email: formData.get("email") as string,
        phone: formData.get("phone") as string,
        role: formData.get("role") as string,
        // add any other fields your UpdateAdminRequest expects:
        securityQuestion: formData.get("securityQuestion") as string,
        securityAnswer: formData.get("securityAnswer") as string,
      }
  
      // Calls PUT /api/admin/users/{id}
      await updateUser(editUser.id, updatedUser)
  
      setIsEditDialogOpen(false)
      fetchUsers()
    } catch (err: any) {
      console.error("Failed to update user:", err)
      alert(`Failed to update user: ${err.message}`)
    } finally {
      setProcessing(false)
    }
  }

  const handleSearch = (e) => {
    setSearchTerm(e.target.value)
  }

  // Add the toggle user status handler function
  const handleToggleUserStatus = async (user) => {
    //if (confirm(`Are you sure you want to ${user.isActive ? "deactivate" : "activate"} this user account?`)) {
      try {
        setLoading(true)
        await toggleUserStatus(user.id, user.isActive)
        //alert(`User account ${user.isActive ? "deactivated" : "activated"} successfully`)
        fetchUsers()
      } catch (err) {
        console.error("Failed to toggle user status:", err)
        alert(`Failed to toggle user status: ${err.message}`)
      } finally {
        setLoading(false)
      }
    //}
  }

  return (
    <div className="space-y-4">
      <div className="flex flex-col sm:flex-row gap-2">
        <div className="relative flex-1">
          <SearchIcon className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
          <Input
            type="search"
            placeholder="Search users..."
            className="w-full pl-8"
            value={searchTerm}
            onChange={handleSearch}
          />
        </div>
        <select
          className="h-10 rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background file:border-0 file:bg-transparent file:text-sm file:font-medium placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
          value={searchType}
          onChange={(e) => setSearchType(e.target.value)}
        >
          <option value="all">All Fields</option>
          <option value="email">Email</option>
          <option value="phone">Phone</option>
        </select>
      </div>

      <div className="rounded-md border">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>User ID</TableHead>
              <TableHead>Name</TableHead>
              <TableHead>Email</TableHead>
              <TableHead>Phone</TableHead>
              <TableHead>Role</TableHead>
              <TableHead>Joined</TableHead>
              <TableHead>Status</TableHead>
              <TableHead className="text-right">Actions</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {loading ? (
              <TableRow>
                <TableCell colSpan={7} className="text-center py-10">
                  Loading users...
                </TableCell>
              </TableRow>
            ) : error ? (
              <TableRow>
                <TableCell colSpan={7} className="text-center py-10 text-destructive">
                  Error: {error}
                </TableCell>
              </TableRow>
            ) : filteredUsers.length === 0 ? (
              <TableRow>
                <TableCell colSpan={7} className="text-center py-10">
                  {searchTerm ? "No users found matching your search." : "No users found."}
                </TableCell>
              </TableRow>
            ) : (
              filteredUsers.map((user) => (
                <TableRow key={user.id}>
                  <TableCell className="font-mono text-xs">{user.id}</TableCell>
                  <TableCell className="font-medium">
                    {user.firstName} {user.lastName}
                  </TableCell>
                  <TableCell>{user.email}</TableCell>
                  <TableCell>{user.phone || "N/A"}</TableCell>
                  <TableCell>
                    <Badge variant={user.role === "Admin" ? "default" : "secondary"}>
                      {user.role === "Investor" ? "User" : user.role}
                    </Badge>
                  </TableCell>
                  <TableCell>{new Date(user.createdAt).toLocaleDateString()}</TableCell>
                  <TableCell>                          
                  <Badge variant={user.isActive ? "success" : "destructive"}>
                    {user.isActive ? "Active" : "Inactive"}
                  </Badge>
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
                        <DropdownMenuItem asChild>
                          <Link href={`/dashboard/users/${user.id}`}>
                            <Eye className="mr-2 h-4 w-4" />
                            View details
                          </Link>
                        </DropdownMenuItem>
                        <DropdownMenuItem onClick={() => handleEditUser(user)}>
                          <Pencil className="mr-2 h-4 w-4" />
                          Edit user
                        </DropdownMenuItem>
                        <DropdownMenuSeparator />
                        <DropdownMenuItem onClick={() => handleToggleUserStatus(user)}>
                          {user.isActive ? (
                            <>
                              <UserX className="mr-2 h-4 w-4" />
                              Deactivate account
                            </>
                          ) : (
                            <>
                              <UserCheck className="mr-2 h-4 w-4" />
                              Activate account
                            </>
                          )}
                        </DropdownMenuItem>
                      </DropdownMenuContent>
                    </DropdownMenu>
                  </TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
      </div>

      <Dialog open={isEditDialogOpen} onOpenChange={setIsEditDialogOpen}>
        <DialogContent className="sm:max-w-[425px]">
          <DialogHeader>
            <DialogTitle>Edit User</DialogTitle>
            <DialogDescription>Make changes to the user profile here. Click save when you're done.</DialogDescription>
          </DialogHeader>
          {editUser && (
            <form onSubmit={handleUpdateUser}>
              <div className="grid gap-4 py-4">
                <div className="grid grid-cols-4 items-center gap-4">
                  <Label htmlFor="userId" className="text-right">
                    User ID
                  </Label>
                  <Input id="userId" value={editUser.id} className="col-span-3" disabled />
                </div>
                <div className="grid grid-cols-4 items-center gap-4">
                  <Label htmlFor="name" className="text-right">
                    Name
                  </Label>
                  <Input
                    id="name"
                    defaultValue={`${editUser.firstName} ${editUser.lastName}`}
                    className="col-span-3"
                    disabled
                  />
                </div>
                <div className="grid grid-cols-4 items-center gap-4">
                  <Label htmlFor="email" className="text-right">
                    Email
                  </Label>
                  <Input id="email" name="email" defaultValue={editUser.email} className="col-span-3" />
                </div>
                <div className="grid grid-cols-4 items-center gap-4">
                  <Label htmlFor="phone" className="text-right">
                    Phone
                  </Label>
                  <Input id="phone" name="phone" defaultValue={editUser.phone} className="col-span-3" />
                </div>
                <div className="grid grid-cols-4 items-center gap-4">
                  <Label htmlFor="role" className="text-right">
                    Role
                  </Label>
                  <select
                    id="role"
                    name="role"
                    defaultValue={editUser.role === "Investor" ? "User" : editUser.role}
                    className="col-span-3 flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background file:border-0 file:bg-transparent file:text-sm file:font-medium placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
                  >
                    <option value="Admin">Admin</option>
                    <option value="User">User</option>
                  </select>
                </div>
                <div className="grid grid-cols-4 items-center gap-4">
                  <Label htmlFor="created" className="text-right">
                    Created
                  </Label>
                  <Input
                    id="created"
                    value={new Date(editUser.createdAt).toLocaleString()}
                    className="col-span-3"
                    disabled
                  />
                </div>
                <div className="grid grid-cols-4 items-center gap-4">
                  <Label htmlFor="updated" className="text-right">
                    Last Updated
                  </Label>
                  <Input
                    id="updated"
                    value={editUser.updatedAt ? new Date(editUser.updatedAt).toLocaleString() : "Never"}
                    className="col-span-3"
                    disabled
                  />
                </div>
              </div>
              <DialogFooter>
                <Button
                  type="button"
                  variant="outline"
                  onClick={() => setIsEditDialogOpen(false)}
                  disabled={processing}
                >
                  Cancel
                </Button>
                <Button type="submit" disabled={processing}>
                  {processing ? "Saving..." : "Save changes"}
                </Button>
              </DialogFooter>
            </form>
          )}
        </DialogContent>
      </Dialog>
    </div>
  )
}
