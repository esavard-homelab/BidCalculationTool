import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest'
import { apiClient, ApiClient } from '@/core/services/ApiClient'

// Mock global fetch
const mockFetch = vi.fn()
global.fetch = mockFetch

describe('ApiClient', () => {
  beforeEach(() => {
    mockFetch.mockClear()
  })

  afterEach(() => {
    vi.restoreAllMocks()
  })

  describe('constructor', () => {
    it('should use default baseUrl when none provided', () => {
      expect(apiClient).toBeDefined()
    })

    it('should use custom baseUrl when provided', () => {
      expect(apiClient).toBeDefined()
    })
  })

  describe('get method', () => {
    it('should make successful GET request and return data', async () => {
      const mockData = { id: 1, name: 'Test' }
      const mockResponse = {
        ok: true,
        json: vi.fn().mockResolvedValue(mockData)
      }
      mockFetch.mockResolvedValue(mockResponse)

      const result = await apiClient.get<typeof mockData>('/test-endpoint')

      expect(mockFetch).toHaveBeenCalledWith('http://localhost:5000/test-endpoint')
      expect(mockResponse.json).toHaveBeenCalled()
      expect(result).toEqual(mockData)
    })

    it('should throw error when GET request fails', async () => {
      const mockResponse = {
        ok: false,
        status: 404
      }
      mockFetch.mockResolvedValue(mockResponse)

      await expect(apiClient.get('/not-found')).rejects.toThrow('HTTP error! status: 404')
      expect(mockFetch).toHaveBeenCalledWith('http://localhost:5000/not-found')
    })

    it('should handle different HTTP error statuses', async () => {
      const mockResponse = {
        ok: false,
        status: 500
      }
      mockFetch.mockResolvedValue(mockResponse)

      await expect(apiClient.get('/server-error')).rejects.toThrow('HTTP error! status: 500')
    })
  })

  describe('post method', () => {
    it('should make successful POST request with data and return response', async () => {
      const postData = { name: 'New Item', value: 123 }
      const responseData = { id: 1, ...postData }
      const mockResponse = {
        ok: true,
        json: vi.fn().mockResolvedValue(responseData)
      }
      mockFetch.mockResolvedValue(mockResponse)

      const result = await apiClient.post('/create', postData)

      expect(mockFetch).toHaveBeenCalledWith('http://localhost:5000/create', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(postData),
      })
      expect(mockResponse.json).toHaveBeenCalled()
      expect(result).toEqual(responseData)
    })

    it('should throw error when POST request fails', async () => {
      const postData = { invalid: 'data' }
      const mockResponse = {
        ok: false,
        status: 422
      }
      mockFetch.mockResolvedValue(mockResponse)

      await expect(apiClient.post('/create', postData)).rejects.toThrow('HTTP error! status: 422')

      expect(mockFetch).toHaveBeenCalledWith('http://localhost:5000/create', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(postData),
      })
    })

    it('should handle different POST error statuses', async () => {
      const postData = { test: 'data' }
      const mockResponse = {
        ok: false,
        status: 401
      }
      mockFetch.mockResolvedValue(mockResponse)

      await expect(apiClient.post('/unauthorized', postData)).rejects.toThrow('HTTP error! status: 401')
    })

    it('should properly serialize complex objects in POST body', async () => {
      const complexData = {
        nested: {
          array: [1, 2, 3],
          object: { key: 'value' }
        },
        nullValue: null,
        boolValue: true
      }
      const mockResponse = {
        ok: true,
        json: vi.fn().mockResolvedValue({ success: true })
      }
      mockFetch.mockResolvedValue(mockResponse)

      await apiClient.post('/complex', complexData)

      expect(mockFetch).toHaveBeenCalledWith('http://localhost:5000/complex', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(complexData),
      })
    })
  })

  describe('network error handling', () => {
    it('should handle fetch network errors in GET', async () => {
      mockFetch.mockRejectedValue(new Error('Network error'))

      await expect(apiClient.get('/network-fail')).rejects.toThrow('Network error')
    })

    it('should handle fetch network errors in POST', async () => {
      mockFetch.mockRejectedValue(new Error('Network error'))

      await expect(apiClient.post('/network-fail', {})).rejects.toThrow('Network error')
    })
  })

  describe('endpoint handling', () => {
    it('should handle endpoints with leading slash', async () => {
      const mockResponse = {
        ok: true,
        json: vi.fn().mockResolvedValue({})
      }
      mockFetch.mockResolvedValue(mockResponse)

      await apiClient.get('/api/data')

      expect(mockFetch).toHaveBeenCalledWith('http://localhost:5000/api/data')
    })

    it('should handle endpoints without leading slash', async () => {
      const mockResponse = {
        ok: true,
        json: vi.fn().mockResolvedValue({})
      }
      mockFetch.mockResolvedValue(mockResponse)

      await apiClient.get('api/data')

      expect(mockFetch).toHaveBeenCalledWith('http://localhost:5000/api/data')
    })
  })

  describe('buildUrl edge cases', () => {
    it('should handle baseUrl with trailing slash and endpoint with leading slash', async () => {
      const client = new ApiClient('http://localhost:5000/')
      const mockResponse = { ok: true, json: vi.fn().mockResolvedValue({}) }
      global.fetch = vi.fn().mockResolvedValue(mockResponse)
      await client.get('/api/test')
      expect(global.fetch).toHaveBeenCalledWith('http://localhost:5000/api/test')
    })

    it('should handle baseUrl with trailing slash and endpoint without leading slash', async () => {
      const client = new ApiClient('http://localhost:5000/')
      const mockResponse = { ok: true, json: vi.fn().mockResolvedValue({}) }
      global.fetch = vi.fn().mockResolvedValue(mockResponse)
      await client.get('api/test')
      expect(global.fetch).toHaveBeenCalledWith('http://localhost:5000/api/test')
    })

    it('should handle baseUrl without trailing slash and endpoint with leading slash', async () => {
      const client = new ApiClient('http://localhost:5000')
      const mockResponse = { ok: true, json: vi.fn().mockResolvedValue({}) }
      global.fetch = vi.fn().mockResolvedValue(mockResponse)
      await client.get('/api/test')
      expect(global.fetch).toHaveBeenCalledWith('http://localhost:5000/api/test')
    })

    it('should handle baseUrl without trailing slash and endpoint without leading slash', async () => {
      const client = new ApiClient('http://localhost:5000')
      const mockResponse = { ok: true, json: vi.fn().mockResolvedValue({}) }
      global.fetch = vi.fn().mockResolvedValue(mockResponse)
      await client.get('api/test')
      expect(global.fetch).toHaveBeenCalledWith('http://localhost:5000/api/test')
    })
  })
})
