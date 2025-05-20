"use client"

import { useState, useEffect } from "react"
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Badge } from "@/components/ui/badge"
import {
  Dialog,
  DialogContent,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogDescription,
} from "@/components/ui/dialog"
import {
  getPropertyById,
  getInvestmentsByProperty,
  togglePropertyInvestment,
} from "@/lib/api"
import { format } from "date-fns"
import { API_BASE_URL } from "@/config"

export function PropertyDetails({ propertyId }: { propertyId: string }) {
  const [property, setProperty] = useState<any>(null)
  const [investments, setInvestments] = useState<any[]>([])
  const [images, setImages] = useState<string[]>([])
  const [loading, setLoading] = useState<boolean>(true)
  const [error, setError] = useState<string | null>(null)

  const [confirmOpen, setConfirmOpen] = useState(false)
  const [confirmMessage, setConfirmMessage] = useState("")
  const [onConfirmAction, setOnConfirmAction] = useState<() => Promise<void>>(
    () => async () => { }
  )

  // Fetch property + images + investments
  useEffect(() => {
    async function fetchPropertyData() {
      setLoading(true)
      try {
        const pd = await getPropertyById(propertyId)
        setProperty(pd)

        const imgRes = await fetch(
          `${API_BASE_URL}/api/property/${propertyId}/images`
        )
        const imgJson = await imgRes.json()
        setImages(imgJson.imageUrls || [])

        try {
          const inv = await getInvestmentsByProperty(propertyId)
          setInvestments(inv)
        } catch {
          setInvestments([])
        }

        setError(null)
      } catch (err: any) {
        setError(err.message)
      } finally {
        setLoading(false)
      }
    }
    fetchPropertyData()
  }, [propertyId])

  // Show confirm dialog
  function askConfirm(message: string, action: () => Promise<void>) {
    setConfirmMessage(message)
    setOnConfirmAction(() => async () => {
      await action()
      setConfirmOpen(false)
    })
    setConfirmOpen(true)
  }

  // Toggle using same API signature as PropertyTable
  const handleToggleInvestment = () => {
    if (!property) return
    const isEnabled = property.investmentsEnabled === true
    const verb = isEnabled ? "disable" : "enable"

    askConfirm(
      `Are you sure you want to ${verb} investment for "${property.name}"?`,
      async () => {
        setLoading(true)
        try {
          // pass the current flag so the API toggles correctly
          await togglePropertyInvestment(propertyId, isEnabled)
          // re-fetch real status
          const updated = await getPropertyById(propertyId)
          setProperty(updated)
        } catch (err: any) {
          setError(err.message)
        } finally {
          setLoading(false)
        }
      }
    )
  }

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary mx-auto" />
          <p className="mt-4 text-muted-foreground">Loading property data…</p>
        </div>
      </div>
    )
  }

  if (error) {
    return (
      <Card>
        <CardHeader>
          <CardTitle className="text-destructive">Error</CardTitle>
          <CardDescription>Failed to load property data</CardDescription>
        </CardHeader>
        <CardContent><p>{error}</p></CardContent>
        <CardFooter>
          <Button variant="outline" onClick={() => window.history.back()}>
            Go Back
          </Button>
        </CardFooter>
      </Card>
    )
  }

  if (!property) {
    return (
      <Card>
        <CardHeader>
          <CardTitle>Not Found</CardTitle>
          <CardDescription>Property not found</CardDescription>
        </CardHeader>
        <CardContent>
          <p>The property with ID {propertyId} does not exist.</p>
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
      {/* Main Details */}
      <Card>
        <CardHeader>
          <CardTitle className="text-2xl">{property.name}</CardTitle>
          <CardDescription>Property ID: {property.id}</CardDescription>
        </CardHeader>
        <CardContent>
          <div className="grid gap-6 md:grid-cols-2">
            <div className="space-y-4">
              {/* Address, location, values, shares */}
              <div>
                <h3 className="text-sm font-medium text-muted-foreground">Address</h3>
                <p>{property.address || "Not provided"}</p>
              </div>
              <div>
                <h3 className="text-sm font-medium text-muted-foreground">Location</h3>
                <p>{property.location}</p>
              </div>
              <div>
                <h3 className="text-sm font-medium text-muted-foreground">Current Value</h3>
                <p>${property.currentValue?.toLocaleString() || "0"}</p>
              </div>
              <div>
                <h3 className="text-sm font-medium text-muted-foreground">Share Price</h3>
                <p>${property.sharePrice?.toLocaleString() || "0"}</p>
              </div>
              <div>
                <h3 className="text-sm font-medium text-muted-foreground">Available Shares</h3>
                <p>{property.availableShares || "0"}</p>
              </div>
              {/* Status & Toggle */}
              <div className="space-y-2">
                <div>
                  <h3 className="text-sm font-medium text-muted-foreground">Status</h3>
                  <div className="flex items-center gap-2 mt-1">

                    <Badge variant={property.investmentsEnabled ? "default" : "secondary"}>
                      {property.investmentsEnabled ? "Investments Enabled" : "NO Investment Allowed"}
                    </Badge>
                  </div>
                </div>
                <Button
                  variant={property.investmentsEnabled ? "destructive" : "default"}
                  size="sm"
                  onClick={handleToggleInvestment}
                >
                  {property.investmentsEnabled ? "Disable Investment" : "Enable Investment"}
                </Button>
              </div>
            </div>
            <div className="space-y-4">
              <div>
                <h3 className="text-sm font-medium text-muted-foreground">Description</h3>
                <p className="whitespace-pre-wrap">{property.description || "No description."}</p>
              </div>
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Images */}
      <Card>
        <CardHeader>
          <CardTitle>Property Images</CardTitle>
          <CardDescription>Gallery</CardDescription>
        </CardHeader>
        <CardContent>
          {images.length > 0 ? (
            <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4">
              {images.map((url, i) => (
                <div key={i} className="aspect-square rounded-md overflow-hidden">
                  <img
                    src={url.startsWith("http") ? url : `${API_BASE_URL}${url}`}
                    alt={`${property.name} image ${i + 1}`}
                    className="h-full w-full object-cover"
                  />
                </div>
              ))}
            </div>
          ) : (
            <div className="text-center py-8 border rounded-md">
              <p className="text-muted-foreground">No images available.</p>
            </div>
          )}
        </CardContent>
      </Card>

      {/* Investments List */}
      <Card>
        <CardHeader>
          <CardTitle>Investments</CardTitle>
          <CardDescription>All investments on this property</CardDescription>
        </CardHeader>
        <CardContent>
          {investments.length === 0 ? (
            <p className="text-center py-4 text-muted-foreground">
              No investments found.
            </p>
          ) : (
            <div className="space-y-4">
              {investments.map((inv) => (
                <div key={inv.id} className="flex items-center justify-between border-b pb-4">
                  <div>
                    <p className="font-medium">User ID: {inv.userId}</p>
                    <p className="text-sm text-muted-foreground">
                      {inv.numOfSharesPurchased} shares @ ${inv.sharePrice} each
                    </p>
                    <p className="text-xs text-muted-foreground">
                      {format(new Date(inv.createdAt), "PPP")}
                    </p>
                  </div>
                  <div className="text-right">
                    <p className="font-medium">
                      ${inv.investmentAmount?.toLocaleString() || "0"}
                    </p>
                    <Badge variant={inv.sellRequest ? "outline" : "default"}>
                      {inv.sellRequest ? "Sell Requested" : "Active"}
                    </Badge>
                  </div>
                </div>
              ))}
            </div>
          )}
        </CardContent>
      </Card>

      {/* Confirm Dialog */}
      <Dialog open={confirmOpen} onOpenChange={setConfirmOpen}>
        <DialogContent className="sm:max-w-[400px]">
          <DialogHeader>
            <DialogTitle>Confirm</DialogTitle>
            <DialogDescription>{confirmMessage}</DialogDescription>
          </DialogHeader>
          <DialogFooter className="flex justify-end space-x-2">
            <Button
              variant="outline"
              onClick={() => setConfirmOpen(false)}
              disabled={loading}
            >
              Cancel
            </Button>
            <Button
              onClick={async () => {
                await onConfirmAction()
              }}
              disabled={loading}
            >
              Confirm
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  )
}
