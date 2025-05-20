"use client"

import { useState, useEffect, ChangeEvent, FormEvent, useRef } from "react"
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
  DialogDescription,
} from "@/components/ui/dialog"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Textarea } from "@/components/ui/textarea"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuTrigger,
  DropdownMenuSeparator,
} from "@/components/ui/dropdown-menu"
import {
  MoreHorizontal,
  Pencil,
  Eye,
  ImagePlus,
  Trash2,
  SearchIcon,
  Ban,
  CheckCircle,
} from "lucide-react"
import {
  getAllProperties,
  getPropertyById,
  togglePropertyInvestment,
  deleteProperty,
} from "@/lib/api"
import Link from "next/link"
import { Badge } from "@/components/ui/badge"
import { API_BASE_URL } from "@/config"

// ✨ Export the interface so it can be reused elsewhere
export interface Property {
  id: string
  name: string
  location: string
  address: string
  description: string
  currentValue?: number
  sharePrice?: number
  availableShares?: number
  investmentsEnabled?: boolean
  createdAt?: string
  images?: string[]
}

export function PropertyTable({
  initialProperties,
  onToggle,
}: {
  initialProperties?: Property[]
  onToggle?: () => Promise<void>
}) {
  const [properties, setProperties] = useState<Property[]>([])
  const [filteredProperties, setFilteredProperties] = useState<Property[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const [editProperty, setEditProperty] = useState<Property | null>(null)
  const [isEditDialogOpen, setIsEditDialogOpen] = useState(false)

  const [isImageDialogOpen, setIsImageDialogOpen] = useState(false)
  const [selectedProperty, setSelectedProperty] = useState<Property | null>(null)
  const [imageUrls, setImageUrls] = useState<string[]>([])
  const [selectedFiles, setSelectedFiles] = useState<File[]>([])
  const [processing, setProcessing] = useState(false)

  const [searchTerm, setSearchTerm] = useState("")
  const [thumbs, setThumbs] = useState<Record<string, string>>({})

  const fileInputRef = useRef<HTMLInputElement>(null)
  const [toDeleteUrls, setToDeleteUrls] = useState<string[]>([])
  const [errorOpen, setErrorOpen] = useState(false)
  const [errorMessage, setErrorMessage] = useState("")

  const [confirmOpen, setConfirmOpen] = useState(false)
  const [confirmMessage, setConfirmMessage] = useState("")
  const [onConfirmAction, setOnConfirmAction] = useState<() => Promise<void>>(
    () => async () => {}
  )

  useEffect(() => {
    if (initialProperties) {
      ;(async () => {
        setLoading(true)
        setProperties(initialProperties)
        setFilteredProperties(initialProperties)
        const map: Record<string, string> = {}
        await Promise.all(
          initialProperties.map(async (p) => {
            const res = await fetch(`${API_BASE_URL}/api/property/${p.id}/images`)
            const json = await res.json()
            map[p.id] = json.imageUrls?.[0] || ""
          })
        )
        setThumbs(map)
        setError(null)
        setLoading(false)
      })()
    } else {
      fetchProperties()
    }
  }, [initialProperties])

  async function fetchProperties() {
    setLoading(true)
    try {
      const data = (await getAllProperties()) as Property[]
      setProperties(data)
      setFilteredProperties(data)
      const map: Record<string, string> = {}
      await Promise.all(
        data.map(async (p) => {
          const res = await fetch(`${API_BASE_URL}/api/property/${p.id}/images`)
          const json = await res.json()
          map[p.id] = json.imageUrls?.[0] || ""
        })
      )
      setThumbs(map)
      setError(null)
    } catch (err: any) {
      setError(err.message)
    } finally {
      setLoading(false)
    }
  }

  function askConfirm(message: string, action: () => Promise<void>) {
    setConfirmMessage(message)
    setOnConfirmAction(() => async () => {
      await action()
      setConfirmOpen(false)
    })
    setConfirmOpen(true)
  }

  function handleSearch(e: ChangeEvent<HTMLInputElement>) {
    setSearchTerm(e.target.value)
  }

  useEffect(() => {
    if (!searchTerm.trim()) {
      setFilteredProperties(properties)
      return
    }
    const term = searchTerm.toLowerCase()
    setFilteredProperties(
      properties.filter((p) =>
        [p.name, p.location, p.address, p.description, p.id]
          .filter(Boolean)
          .some((f) => f.toLowerCase().includes(term))
      )
    )
  }, [searchTerm, properties])

  function handleDeleteProperty(p: Property) {
    askConfirm(
      `Delete "${p.name}"? This cannot be undone.`,
      async () => {
        setProcessing(true)
        try {
          await deleteProperty(p.id)
          if (initialProperties && onToggle) {
            await onToggle()
          } else {
            await fetchProperties()
          }
        } catch (err: any) {
          // show our new error dialog
          setErrorMessage(`Error deleting property:\n${err.message}`)
          setErrorOpen(true)
        } finally {
          setProcessing(false)
        }
      }
    )
  }
  
  
  // ───────────────────────────────────

  function handleEditProperty(p: Property) {
    setEditProperty(p)
    setIsEditDialogOpen(true)
  }

  async function handleManageImages(p: Property) {
    setProcessing(true)
    try {
      const full = (await getPropertyById(p.id)) as Property
      setSelectedProperty(full)
      const res = await fetch(`${API_BASE_URL}/api/property/${p.id}/images`)
      const json = await res.json()
      setImageUrls(json.imageUrls || [])
      setSelectedFiles([])
      setIsImageDialogOpen(true)
    } finally {
      setProcessing(false)
    }
  }

  function handleFileChange(e: ChangeEvent<HTMLInputElement>) {
    if (!e.target.files) return
    const newFiles = Array.from(e.target.files)
    setSelectedFiles((prev) => [...prev, ...newFiles])
    e.target.value = ""
  }

  function removeSelectedFile(i: number) {
    setSelectedFiles((prev) => prev.filter((_, idx) => idx !== i))
  }

  async function handleUpdateProperty(e: FormEvent<HTMLFormElement>) {
    e.preventDefault()
    setProcessing(true)
    try {
      const form = new FormData(e.currentTarget)
      const payload = {
        Name: form.get("name"),
        Address: form.get("address"),
        Location: form.get("location"),
        CurrentValue: form.get("currentValue")
          ? parseFloat(form.get("currentValue") as string)
          : undefined,
        AvailableShares: form.get("availableShares")
          ? parseInt(form.get("availableShares") as string, 10)
          : undefined,
        Description: form.get("description"),
      }

      const resp = await fetch(
        `${API_BASE_URL}/api/admin/properties/${editProperty!.id}`,
        {
          method: "PUT",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(payload),
        }
      )
      if (!resp.ok) throw new Error(await resp.text())
      setIsEditDialogOpen(false)
      if (!initialProperties) {
        fetchProperties()
      }
    } finally {
      setProcessing(false)
    }
  }

  async function handleUploadImages(e: FormEvent) {
    e.preventDefault()
    if (!selectedFiles.length) return
    setProcessing(true)
    try {
      const fd = new FormData()
      selectedFiles.forEach((f) => fd.append("files", f))
      const resp = await fetch(
        `${API_BASE_URL}/api/property/${selectedProperty!.id}/upload-images`,
        { method: "POST", body: fd }
      )
      if (!resp.ok) throw new Error(await resp.text())

      const res2 = await fetch(
        `${API_BASE_URL}/api/property/${selectedProperty!.id}/images`
      )
      const json2 = await res2.json()
      setImageUrls(json2.imageUrls || [])
      if (!initialProperties) {
        fetchProperties()
      }

      setSelectedFiles([])
      if (fileInputRef.current) fileInputRef.current.value = ""
    } finally {
      setProcessing(false)
    }
  }

  async function handleDeleteImage(url: string) {
    askConfirm("Delete this image?", async () => {
      setProcessing(true)
      try {
        const fileName = url.split("/").pop()!
        const resp = await fetch(
          `${API_BASE_URL}/api/property/${selectedProperty!.id}/images?fileNames=${encodeURIComponent(
            fileName
          )}`,
          { method: "DELETE" }
        )
        if (!resp.ok) throw new Error(await resp.text())

        const listResp = await fetch(
          `${API_BASE_URL}/api/property/${selectedProperty!.id}/images`
        )
        const { imageUrls: updated } = await listResp.json()
        setImageUrls(updated)
        if (!initialProperties) {
          fetchProperties()
        }
      } finally {
        setProcessing(false)
      }
    })
  }

  async function handleDeleteSelectedImages() {
    if (!toDeleteUrls.length) return

    askConfirm(`Delete ${toDeleteUrls.length} image(s)?`, async () => {
      setProcessing(true)
      try {
        const params = toDeleteUrls
          .map((u) => `fileNames=${encodeURIComponent(u.split("/").pop()!)}`)
          .join("&")

        const resp = await fetch(
          `${API_BASE_URL}/api/property/${selectedProperty!.id}/images?${params}`,
          { method: "DELETE" }
        )
        if (!resp.ok) throw new Error(await resp.text())

        const listResp = await fetch(
          `${API_BASE_URL}/api/property/${selectedProperty!.id}/images`
        )
        const { imageUrls: updated } = await listResp.json()
        setImageUrls(updated)
        if (!initialProperties) {
          fetchProperties()
        }
        setToDeleteUrls([])
      } finally {
        setProcessing(false)
      }
    })
  }

  async function handleToggleInvestment(p: Property) {
    const isCurrentlyEnabled = p.investmentsEnabled === true
    const actionText = isCurrentlyEnabled ? "disable" : "enable"

    askConfirm(
      `Are you sure you want to ${actionText} investments for "${p.name}"?`,
      async () => {
        setLoading(true)
        try {
          await togglePropertyInvestment(p.id, isCurrentlyEnabled)
          if (initialProperties && onToggle) {
            await onToggle()
          } else {
            fetchProperties()
          }
        } finally {
          setLoading(false)
        }
      }
    )
  }

  return (
    <div className="space-y-4">
      {/* SEARCH */}
      <div className="relative">
        <SearchIcon className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
        <Input
          type="search"
          placeholder="Search properties..."
          className="w-full pl-8"
          value={searchTerm}
          onChange={handleSearch}
        />
      </div>

      {/* TABLE */}
      <div className="rounded-md border">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Image</TableHead>
              <TableHead>Name</TableHead>
              <TableHead>Location</TableHead>
              <TableHead>Current Value</TableHead>
              <TableHead>Share Price</TableHead>
              <TableHead>Available Shares</TableHead>
              <TableHead>Status</TableHead>
              <TableHead className="text-right">Actions</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {loading ? (
              <TableRow>
                <TableCell colSpan={8} className="text-center py-10">
                  Loading properties...
                </TableCell>
              </TableRow>
            ) : error ? (
              <TableRow>
                <TableCell colSpan={8} className="text-center py-10 text-destructive">
                  {error}
                </TableCell>
              </TableRow>
            ) : filteredProperties.length === 0 ? (
              <TableRow>
                <TableCell colSpan={8} className="text-center py-10">
                  No properties found.
                </TableCell>
              </TableRow>
            ) : (
              filteredProperties.map((p) => (
                <TableRow key={p.id}>
                  <TableCell>
                    <div className="h-12 w-12 rounded-md overflow-hidden bg-muted">
                      {thumbs[p.id] ? (
                        <img
                          src={
                            thumbs[p.id].startsWith("http")
                              ? thumbs[p.id]
                              : `${API_BASE_URL}${thumbs[p.id]}`
                          }
                          alt={p.name}
                          className="h-full w-full object-cover"
                        />
                      ) : (
                        <div className="flex h-full w-full items-center justify-center text-xs text-muted-foreground">
                          No img
                        </div>
                      )}
                    </div>
                  </TableCell>
                  <TableCell className="font-medium">{p.name}</TableCell>
                  <TableCell>{p.location}</TableCell>
                  <TableCell>${p.currentValue?.toLocaleString() || 0}</TableCell>
                  <TableCell>${p.sharePrice?.toLocaleString() || 0}</TableCell>
                  <TableCell>{p.availableShares || 0}</TableCell>
                  <TableCell>
                    <Badge
                      variant={p.investmentsEnabled !== false ? "default" : "secondary"}
                    >
                      {p.investmentsEnabled !== false ? "Active" : "Disabled"}
                    </Badge>
                  </TableCell>
                  <TableCell className="text-right">
                    <DropdownMenu>
                      <DropdownMenuTrigger asChild>
                        <Button variant="ghost" className="h-8 w-8 p-0">
                          <MoreHorizontal className="h-4 w-4" />
                        </Button>
                      </DropdownMenuTrigger>
                      <DropdownMenuContent align="end">
                        <DropdownMenuLabel>Actions</DropdownMenuLabel>
                        <DropdownMenuItem asChild>
                          <Link href={`/dashboard/properties/${p.id}`}>
                            <Eye className="mr-2 h-4 w-4" /> View Details
                          </Link>
                        </DropdownMenuItem>
                        <DropdownMenuItem onClick={() => handleEditProperty(p)}>
                          <Pencil className="mr-2 h-4 w-4" /> Edit
                        </DropdownMenuItem>
                        <DropdownMenuItem onClick={() => handleManageImages(p)}>
                          <ImagePlus className="mr-2 h-4 w-4" /> Images
                        </DropdownMenuItem>
                        <DropdownMenuSeparator />
                        <DropdownMenuItem onClick={() => handleToggleInvestment(p)}>
                          {p.investmentsEnabled !== false ? (
                            <Ban className="mr-2 h-4 w-4" />
                          ) : (
                            <CheckCircle className="mr-2 h-4 w-4" />
                          )}
                          {p.investmentsEnabled !== false
                            ? "Disable Investments"
                            : "Enable Investments"}
                        </DropdownMenuItem>
                        <DropdownMenuSeparator />
                        <DropdownMenuItem onClick={() => handleDeleteProperty(p)}>
                          <Trash2 className="mr-2 h-4 w-4" /> Delete Property
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

      {/* EDIT PROPERTY DIALOG */}
      <Dialog open={isEditDialogOpen} onOpenChange={() => setIsEditDialogOpen(false)}>
        <DialogContent className="sm:max-w-[600px]">
          <DialogHeader>
            <DialogTitle>Edit Property</DialogTitle>
            <DialogDescription>Update details</DialogDescription>
          </DialogHeader>
          <form onSubmit={handleUpdateProperty}>
            <div className="grid gap-4 py-4">
              <div className="grid grid-cols-4 items-center gap-4">
                <Label htmlFor="propertyId" className="text-right">ID</Label>
                <Input id="propertyId" value={editProperty?.id} className="col-span-3" disabled />
              </div>
              <div className="grid grid-cols-4 items-center gap-4">
                <Label htmlFor="name" className="text-right">Name</Label>
                <Input id="name" name="name" defaultValue={editProperty?.name} className="col-span-3" />
              </div>
              <div className="grid grid-cols-4 items-center gap-4">
                <Label htmlFor="address" className="text-right">Address</Label>
                <Input id="address" name="address" defaultValue={editProperty?.address} className="col-span-3" />
              </div>
              <div className="grid grid-cols-4 items-center gap-4">
                <Label htmlFor="location" className="text-right">Location</Label>
                <Input id="location" name="location" defaultValue={editProperty?.location} className="col-span-3" />
              </div>
              <div className="grid grid-cols-4 items-center gap-4">
                <Label htmlFor="currentValue" className="text-right">Value</Label>
                <Input id="currentValue" name="currentValue" type="number" step="0.01" defaultValue={editProperty?.currentValue} className="col-span-3" />
              </div>
              <div className="grid grid-cols-4 items-center gap-4">
                <Label htmlFor="sharePrice" className="text-right">Share Price</Label>
                <Input id="sharePrice" name="sharePrice" type="number" step="0.01" defaultValue={editProperty?.sharePrice} className="col-span-3" />
              </div>
              <div className="grid grid-cols-4 items-center gap-4">
                <Label htmlFor="availableShares" className="text-right">Shares</Label>
                <Input id="availableShares" name="availableShares" type="number" defaultValue={editProperty?.availableShares} className="col-span-3" />
              </div>
              <div className="grid grid-cols-4 items-center gap-4">
                <Label htmlFor="description" className="text-right">Description</Label>
                <Textarea id="description" name="description" defaultValue={editProperty?.description} rows={4} className="col-span-3" />
              </div>
            </div>
            <DialogFooter>
              <Button variant="outline" disabled={processing}>Cancel</Button>
              <Button type="submit" disabled={processing}>Save</Button>
            </DialogFooter>
          </form>
        </DialogContent>
      </Dialog>

      {/* IMAGE MANAGEMENT DIALOG */}
      <Dialog open={isImageDialogOpen} onOpenChange={() => setIsImageDialogOpen(false)}>
        <DialogContent className="sm:max-w-[600px]">
          <DialogHeader>
            <DialogTitle>Manage Images</DialogTitle>
            <DialogDescription>Upload or delete images only png,webp,jpg Images allowed </DialogDescription>
          </DialogHeader>
          <form onSubmit={handleUploadImages} className="space-y-4">
            <Input
              ref={fileInputRef}
              type="file"
              multiple
              accept="image/*"
              onChange={handleFileChange}
            />

            {selectedFiles.length > 0 && (
              <div>
                <p className="text-sm font-medium">
                  {selectedFiles.length} file{selectedFiles.length > 1 ? "s" : ""} ready to upload:
                </p>
                <div className="grid grid-cols-3 gap-4">
                  {selectedFiles.map((file, idx) => (
                    <div key={idx} className="relative">
                      <img
                        src={URL.createObjectURL(file)}
                        className="rounded-md w-full h-32 object-cover"
                      />
                      <Button
                        type="button"
                        variant="outline"
                        size="sm"
                        className="absolute top-1 right-1 p-1"
                        onClick={() => removeSelectedFile(idx)}
                      >
                        ✕
                      </Button>
                    </div>
                  ))}
                </div>
              </div>
            )}

            <div>
              <p className="text-sm font-medium mb-2">
                Already uploaded (select to delete):
              </p>
              <div className="grid grid-cols-3 gap-4">
                {imageUrls.map((url) => {
                  const checked = toDeleteUrls.includes(url)
                  return (
                    <label key={url} className="relative block">
                      <input
                        type="checkbox"
                        className="absolute top-1 left-1 z-10 h-4 w-4"
                        checked={checked}
                        onChange={() =>
                          setToDeleteUrls((prev) =>
                            checked ? prev.filter((u) => u !== url) : [...prev, url]
                          )
                        }
                      />
                      <img
                        src={url.startsWith("http") ? url : `${API_BASE_URL}${url}`}
                        className="rounded-md w-full h-32 object-cover"
                      />
                    </label>
                  )
                })}
              </div>
            </div>

            <DialogFooter className="flex justify-between">
              <Button
                type="button"
                variant="destructive"
                disabled={processing || !toDeleteUrls.length}
                onClick={handleDeleteSelectedImages}
              >
                Delete Selected
              </Button>
              <div className="space-x-2">
                <Button
                  type="button"
                  variant="outline"
                  disabled={processing}
                  onClick={() => {
                    setIsImageDialogOpen(false)
                    setSelectedFiles([])
                    if (fileInputRef.current) fileInputRef.current.value = ""
                    setToDeleteUrls([])
                  }}
                >
                  Cancel
                </Button>
                <Button type="submit" disabled={processing || !selectedFiles.length}>
                  Upload
                </Button>
              </div>
            </DialogFooter>
          </form>
        </DialogContent>
      </Dialog>

      {/* Delete DIALOG */}
      <Dialog open={errorOpen} onOpenChange={setErrorOpen}>
  <DialogContent className="sm:max-w-[400px]">
    <DialogHeader>
      <DialogTitle>Error</DialogTitle>
      <DialogDescription>{errorMessage}</DialogDescription>
    </DialogHeader>
    <DialogFooter className="flex justify-end">
      <Button variant="outline" onClick={() => setErrorOpen(false)}>
        OK
      </Button>
    </DialogFooter>
  </DialogContent>
</Dialog>

      {/* CONFIRM DIALOG */}
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
              disabled={processing}
            >
              Cancel
            </Button>
            <Button
              onClick={async () => {
                await onConfirmAction()
              }}
              disabled={processing}
            >
              Confirm
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  )
}
