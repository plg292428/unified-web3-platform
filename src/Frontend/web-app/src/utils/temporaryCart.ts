/**
 * Temporary Cart Utility
 * Manages shopping cart items in localStorage for unauthenticated users
 */

export interface TemporaryCartItem {
  productId: number
  quantity: number
  addedAt: number // timestamp
}

const STORAGE_KEY = 'temporary_cart'

/**
 * Get all items from temporary cart
 */
export function getTemporaryCartItems(): TemporaryCartItem[] {
  try {
    const stored = localStorage.getItem(STORAGE_KEY)
    if (!stored) {
      return []
    }
    const items: TemporaryCartItem[] = JSON.parse(stored)
    // Filter out items older than 7 days
    const sevenDaysAgo = Date.now() - 7 * 24 * 60 * 60 * 1000
    const validItems = items.filter(item => item.addedAt > sevenDaysAgo)
    if (validItems.length !== items.length) {
      saveTemporaryCartItems(validItems)
    }
    return validItems
  } catch (error) {
    console.error('Failed to get temporary cart items:', error)
    return []
  }
}

/**
 * Save items to temporary cart
 */
function saveTemporaryCartItems(items: TemporaryCartItem[]): void {
  try {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(items))
  } catch (error) {
    console.error('Failed to save temporary cart items:', error)
  }
}

/**
 * Add item to temporary cart
 */
export function addToTemporaryCart(productId: number, quantity = 1): TemporaryCartItem[] {
  const items = getTemporaryCartItems()
  const existingIndex = items.findIndex(item => item.productId === productId)
  
  if (existingIndex >= 0) {
    // Update quantity if item already exists
    items[existingIndex].quantity += quantity
    items[existingIndex].addedAt = Date.now()
  } else {
    // Add new item
    items.push({
      productId,
      quantity,
      addedAt: Date.now()
    })
  }
  
  saveTemporaryCartItems(items)
  return items
}

/**
 * Update item quantity in temporary cart
 */
export function updateTemporaryCartItem(productId: number, quantity: number): TemporaryCartItem[] {
  const items = getTemporaryCartItems()
  const existingIndex = items.findIndex(item => item.productId === productId)
  
  if (existingIndex >= 0) {
    if (quantity <= 0) {
      // Remove item if quantity is 0 or less
      items.splice(existingIndex, 1)
    } else {
      items[existingIndex].quantity = quantity
      items[existingIndex].addedAt = Date.now()
    }
  }
  
  saveTemporaryCartItems(items)
  return items
}

/**
 * Remove item from temporary cart
 */
export function removeFromTemporaryCart(productId: number): TemporaryCartItem[] {
  const items = getTemporaryCartItems()
  const filtered = items.filter(item => item.productId !== productId)
  saveTemporaryCartItems(filtered)
  return filtered
}

/**
 * Clear temporary cart
 */
export function clearTemporaryCart(): void {
  try {
    localStorage.removeItem(STORAGE_KEY)
  } catch (error) {
    console.error('Failed to clear temporary cart:', error)
  }
}

/**
 * Get total quantity of items in temporary cart
 */
export function getTemporaryCartTotalQuantity(): number {
  const items = getTemporaryCartItems()
  return items.reduce((sum, item) => sum + item.quantity, 0)
}

/**
 * Transfer temporary cart items to user account
 * Returns the items that were transferred
 */
export function transferTemporaryCartToUser(): TemporaryCartItem[] {
  const items = getTemporaryCartItems()
  clearTemporaryCart()
  return items
}

