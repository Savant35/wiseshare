"use client"

import { useEffect, useState } from "react"
import { Badge } from "@/components/ui/badge"
import { getAllPayments, getUserById } from "@/lib/api"

export function RecentTransactions() {
  const [transactions, setTransactions] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)
  const [userCache, setUserCache] = useState({})

  useEffect(() => {
    async function fetchTransactions() {
      try {
        const data = await getAllPayments()
        // Get the 5 most recent transactions
        const recentTransactions = [...data]
          .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
          .slice(0, 5)

        // Fetch user details for each transaction
        const userIds = [...new Set(recentTransactions.map((t) => t.userId))]
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
        setTransactions(recentTransactions)
        setError(null)
      } catch (err) {
        console.error("Failed to fetch recent transactions:", err)
        setError(err.message)
      } finally {
        setLoading(false)
      }
    }

    fetchTransactions()
  }, [])

  if (loading) {
    return <div className="text-center py-4">Loading recent transactions...</div>
  }

  if (error) {
    return <div className="text-center py-4 text-destructive">Error: {error}</div>
  }

  if (transactions.length === 0) {
    return <div className="text-center py-4">No transactions found.</div>
  }

  return (
    <div className="space-y-8">
      {transactions.map((transaction) => {
        const user = userCache[transaction.userId]
        return (
          <div key={transaction.id} className="flex items-center">
            <div className="space-y-1">
              <p className="text-sm font-medium leading-none">
                {user ? `${user.firstName} ${user.lastName}` : `User ID: ${transaction.userId}`}
              </p>
              <p className="text-sm text-muted-foreground">
                {user ? user.email : new Date(transaction.createdAt).toLocaleDateString()}
              </p>
            </div>
            <div className="ml-auto flex items-center gap-2">
              <div className="text-sm text-right">
                <p className="font-medium">${transaction.amount.toLocaleString()}</p>
                <p className="text-xs text-muted-foreground">{transaction.type}</p>
              </div>
              <Badge variant={transaction.status === "Completed" ? "default" : "outline"}>{transaction.status}</Badge>
            </div>
          </div>
        )
      })}
    </div>
  )
}
