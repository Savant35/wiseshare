// components/AddPropertyDialog.tsx
"use client"

import type React from "react"
import { useState } from "react"
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Textarea } from "@/components/ui/textarea"
import { API_BASE_URL } from "@/config"

export function AddPropertyDialog({ children }: { children: React.ReactNode }) {
  const [open, setOpen] = useState(false)
  const [processing, setProcessing] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault()
    setError(null)
    setProcessing(true)

    try {
      const formData = new FormData(e.currentTarget)
      const propertyData = {
        name: formData.get("name") as string,
        address: formData.get("address") as string,
        location: formData.get("location") as string,
        originalValue: parseFloat(formData.get("currentValue") as string),
        description: formData.get("description") as string,
      }

      const response = await fetch(`${API_BASE_URL}/api/property/create`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(propertyData),
      })

      if (!response.ok) {
        const text = await response.text()
        throw new Error(text || "Failed to create property")
      }

      setOpen(false)
      window.location.reload()
    } catch (err: any) {
      console.error("Failed to create property:", err)
      setError(err.message || "Failed to create property")
    } finally {
      setProcessing(false)
    }
  }

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>{children}</DialogTrigger>
      <DialogContent className="sm:max-w-[600px]">
        <DialogHeader>
          <DialogTitle>Add New Property</DialogTitle>
          <DialogDescription>
            Create a new property listing. All fields are required.
          </DialogDescription>
        </DialogHeader>
        <form onSubmit={handleSubmit}>
          <div className="grid gap-4 py-4">
            <div className="grid grid-cols-4 items-center gap-4">
              <Label htmlFor="name" className="text-right">Name</Label>
              <Input id="name" name="name" className="col-span-3" required />
            </div>
            <div className="grid grid-cols-4 items-center gap-4">
              <Label htmlFor="address" className="text-right">Address</Label>
              <Input id="address" name="address" className="col-span-3" required />
            </div>
            <div className="grid grid-cols-4 items-center gap-4">
              <Label htmlFor="location" className="text-right">Location</Label>
              <Input id="location" name="location" className="col-span-3" required />
            </div>
            <div className="grid grid-cols-4 items-center gap-4">
              <Label htmlFor="currentValue" className="text-right">Original Value</Label>
              <Input
                id="currentValue"
                name="currentValue"
                type="number"
                step="0.01"
                min="0"
                className="col-span-3"
                required
              />
            </div>
            <div className="grid grid-cols-4 items-center gap-4">
              <Label htmlFor="description" className="text-right">Description</Label>
              <Textarea id="description" name="description" className="col-span-3" rows={4} required />
            </div>
            {error && <p className="text-sm text-destructive text-center">{error}</p>}
          </div>
          <DialogFooter>
            <Button type="button" variant="outline" onClick={() => setOpen(false)} disabled={processing}>
              Cancel
            </Button>
            <Button type="submit" disabled={processing}>
              {processing ? "Creating..." : "Create Property"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  )
}
