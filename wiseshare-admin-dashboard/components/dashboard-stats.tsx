"use client"

import { useEffect, useState } from "react"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Users, Home, CreditCard, TrendingUp } from "lucide-react"
import { getAllUsers, getAllProperties, getAllPayments, getInvestmentsByProperty } from "@/lib/api"

export function DashboardStats() {
  const [stats, setStats] = useState({
    totalUsers: 0,
    totalProperties: 0,
    totalRevenue: 0,
    totalInvestments: 0,
    loading: true,
    error: null,
  })

  useEffect(() => {
    async function fetchStats() {
      try {
        // Fetch all users
        const users = await getAllUsers()

        // Fetch all properties
        const properties = await getAllProperties()

        // Fetch all payments to calculate revenue
        const payments = await getAllPayments()
        const revenue = payments.reduce((total, payment) => {
          if (payment.type === "Deposit" && payment.status === "Completed") {
            return total + payment.amount
          }
          return total
        }, 0)

        // Fetch investments for each property (limited to first 5 to avoid too many requests)
        let allInvestments = []
        for (const property of properties.slice(0, 5)) {
          try {
            const propertyInvestments = await getInvestmentsByProperty(property.id)
            allInvestments = [...allInvestments, ...propertyInvestments]
          } catch (err) {
            console.error(`Failed to fetch investments for property ${property.id}:`, err)
          }
        }

        setStats({
          totalUsers: users.length,
          totalProperties: properties.length,
          totalRevenue: revenue,
          totalInvestments: allInvestments.length,
          loading: false,
          error: null,
        })
      } catch (err) {
        console.error("Failed to fetch dashboard stats:", err)
        setStats((prev) => ({
          ...prev,
          loading: false,
          error: err.message,
        }))
      }
    }

    fetchStats()
  }, [])

  if (stats.loading) {
    return (
      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
        {[1, 2, 3, 4].map((i) => (
          <Card key={i} className="animate-pulse">
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium bg-muted h-4 w-24 rounded"></CardTitle>
              <div className="h-4 w-4 bg-muted rounded"></div>
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold bg-muted h-8 w-20 rounded"></div>
              <div className="text-xs text-muted-foreground bg-muted h-4 w-32 rounded mt-1"></div>
            </CardContent>
          </Card>
        ))}
      </div>
    )
  }

  if (stats.error) {
    return (
      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
        <Card className="col-span-full">
          <CardContent className="pt-6">
            <p className="text-center text-destructive">Error loading dashboard stats: {stats.error}</p>
          </CardContent>
        </Card>
      </div>
    )
  }

  return (
    <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
      <Card>
        <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
          <CardTitle className="text-sm font-medium">Total Users</CardTitle>
          <Users className="h-4 w-4 text-muted-foreground" />
        </CardHeader>
        <CardContent>
          <div className="text-2xl font-bold">{stats.totalUsers.toLocaleString()}</div>
          <p className="text-xs text-muted-foreground">Active users on the platform</p>
        </CardContent>
      </Card>
      <Card>
        <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
          <CardTitle className="text-sm font-medium">Properties</CardTitle>
          <Home className="h-4 w-4 text-muted-foreground" />
        </CardHeader>
        <CardContent>
          <div className="text-2xl font-bold">{stats.totalProperties.toLocaleString()}</div>
          <p className="text-xs text-muted-foreground">Available for investment</p>
        </CardContent>
      </Card>
      <Card>
        <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
          <CardTitle className="text-sm font-medium">Total Revenue</CardTitle>
          <CreditCard className="h-4 w-4 text-muted-foreground" />
        </CardHeader>
        <CardContent>
          <div className="text-2xl font-bold">${stats.totalRevenue.toLocaleString()}</div>
          <p className="text-xs text-muted-foreground">From all completed deposits</p>
        </CardContent>
      </Card>
      <Card>
        <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
          <CardTitle className="text-sm font-medium">Active Investments</CardTitle>
          <TrendingUp className="h-4 w-4 text-muted-foreground" />
        </CardHeader>
        <CardContent>
          <div className="text-2xl font-bold">{stats.totalInvestments.toLocaleString()}</div>
          <p className="text-xs text-muted-foreground">Across all properties</p>
        </CardContent>
      </Card>
    </div>
  )
}
