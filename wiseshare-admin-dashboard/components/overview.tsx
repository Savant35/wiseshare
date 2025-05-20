"use client"

import { useEffect, useState } from "react"
import { Bar, BarChart, ResponsiveContainer, XAxis, YAxis, Tooltip } from "recharts"
import { getAllPayments } from "@/lib/api"

export function Overview() {
  const [data, setData] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)

  useEffect(() => {
    async function fetchData() {
      try {
        const payments = await getAllPayments()

        // Initialize monthly data
        const monthlyData = Array(12)
          .fill()
          .map((_, i) => ({
            name: new Date(0, i).toLocaleString("default", { month: "short" }),
            total: 0,
          }))

        // Calculate total investments per month
        payments.forEach((payment) => {
          if (payment.type === "Deposit" && payment.status === "Completed") {
            const date = new Date(payment.createdAt)
            const monthIndex = date.getMonth()
            monthlyData[monthIndex].total += payment.amount
          }
        })

        setData(monthlyData)
        setError(null)
      } catch (err) {
        console.error("Failed to fetch payment data for chart:", err)
        setError(err.message)
      } finally {
        setLoading(false)
      }
    }

    fetchData()
  }, [])

  if (loading) {
    return <div className="flex items-center justify-center h-[350px]">Loading chart data...</div>
  }

  if (error) {
    return (
      <div className="flex items-center justify-center h-[350px] text-destructive">Error loading chart: {error}</div>
    )
  }

  return (
    <ResponsiveContainer width="100%" height={350}>
      <BarChart data={data}>
        <XAxis dataKey="name" stroke="#888888" fontSize={12} tickLine={false} axisLine={false} />
        <YAxis
          stroke="#888888"
          fontSize={12}
          tickLine={false}
          axisLine={false}
          tickFormatter={(value) => `$${value}`}
        />
        <Tooltip
          formatter={(value) => [`$${value}`, "Total Investment"]}
          labelFormatter={(label) => `Month: ${label}`}
        />
        <Bar dataKey="total" fill="currentColor" radius={[4, 4, 0, 0]} className="fill-primary" />
      </BarChart>
    </ResponsiveContainer>
  )
}
