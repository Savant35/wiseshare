// components/RecentProperties.tsx
"use client"

import { useState, useEffect, useCallback } from "react"
import { getAllProperties } from "@/lib/api"
import { PropertyTable, Property } from "@/components/property-table"

export function RecentProperties() {
  const [recentProps, setRecentProps] = useState<Property[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const fetchRecent = useCallback(async () => {
    setLoading(true)
    try {
      const all = (await getAllProperties()) as Property[]
      const sorted = all
        .sort(
          (a, b) =>
            new Date(b.createdAt || "").getTime() -
            new Date(a.createdAt || "").getTime()
        )
        .slice(0, 5)
      setRecentProps(sorted)
      setError(null)
    } catch (err: any) {
      setError(err.message)
    } finally {
      setLoading(false)
    }
  }, [])

  useEffect(() => {
    fetchRecent()
  }, [fetchRecent])

  if (loading) return <div className="text-center py-4">Loading...</div>
  if (error) return <div className="text-center py-4 text-destructive">{error}</div>
  if (!recentProps.length) return <div className="text-center py-4">No properties.</div>

  return (
    <PropertyTable
      initialProperties={recentProps}
      onToggle={fetchRecent}
    />
  )
}
