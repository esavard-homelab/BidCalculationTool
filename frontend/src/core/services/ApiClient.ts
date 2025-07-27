/**
 * HTTP client for making API requests to the backend.
 * Provides a centralized way to handle API communication with proper error handling.
 */
class ApiClient {
  private readonly baseUrl: string

  /**
   * Creates a new ApiClient instance.
   *
   * @param baseUrl - The base URL for the API (defaults to localhost:5000)
   */
  constructor(baseUrl: string = 'http://localhost:5000') {
    this.baseUrl = baseUrl
  }

  /**
   * Performs a GET request to the specified endpoint.
   *
   * @template T - The expected response type
   * @param endpoint - The API endpoint path (e.g., '/api/vehicletypes')
   * @returns Promise that resolves to the parsed JSON response
   * @throws Error when the HTTP request fails or returns a non-ok status
   *
   * @example
   * ```typescript
   * const vehicleTypes = await apiClient.get<string[]>('/api/vehicletypes')
   * ```
   */
  async get<T>(endpoint: string): Promise<T> {
    const response = await fetch(`${this.baseUrl}${endpoint}`)

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`)
    }

    return await response.json()
  }

  /**
   * Performs a POST request to the specified endpoint with JSON data.
   *
   * @template T - The type of data being sent
   * @template R - The expected response type
   * @param endpoint - The API endpoint path (e.g., '/api/bidcalculation')
   * @param data - The data to send in the request body
   * @returns Promise that resolves to the parsed JSON response
   * @throws Error when the HTTP request fails or returns a non-ok status
   *
   * @example
   * ```typescript
   * const result = await apiClient.post<BidCalculationRequestDto, BidCalculationResponseDto>(
   *   '/api/bidcalculation',
   *   { vehiclePrice: 1000, vehicleType: 'Common' }
   * )
   * ```
   */
  async post<T, R>(endpoint: string, data: T): Promise<R> {
    const response = await fetch(`${this.baseUrl}${endpoint}`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(data),
    })

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`)
    }

    return await response.json()
  }
}

/**
 * Singleton instance of ApiClient for use throughout the application.
 * Pre-configured with the default backend URL.
 */
export const apiClient = new ApiClient()
