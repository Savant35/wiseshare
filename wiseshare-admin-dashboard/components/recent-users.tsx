"use client"

import { useEffect, useState } from "react"
import { Badge } from "@/components/ui/badge"
import { getAllUsers } from "@/lib/api"

export function RecentUsers() {
  const [users, setUsers] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)

  useEffect(() => {
    async function fetchUsers() {
      try {
        const data = await getAllUsers()
        // Get the 5 most recent users
        const recentUsers = [...data]
          .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
          .slice(0, 5)
        setUsers(recentUsers)
        setError(null)
      } catch (err) {
        console.error("Failed to fetch recent users:", err)
        setError(err.message)
      } finally {
        setLoading(false)
      }
    }

    fetchUsers()
  }, [])

  if (loading) {
    return <div className="text-center py-4">Loading recent users...</div>
  }

  if (error) {
    return <div className="text-center py-4 text-destructive">Error: {error}</div>
  }

  if (users.length === 0) {
    return <div className="text-center py-4">No users found.</div>
  }

  return (
    <div className="space-y-8">
      {users.map((user) => (
        <div key={user.id} className="flex items-center">
          <div className="space-y-1">
            <p className="text-sm font-medium leading-none">
              {user.firstName} {user.lastName}
            </p>
            <p className="text-sm text-muted-foreground">{user.email}</p>
          </div>
          <div className="ml-auto flex items-center gap-2">
            <Badge variant={user.role === "Admin" ? "default" : "secondary"}>
              {user.role === "Investor" ? "User" : user.role}
            </Badge>
          </div>
        </div>
      ))}
    </div>
  )
}
