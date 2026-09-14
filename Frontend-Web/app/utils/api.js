import axios from "axios";
import useAuthStore from "@/store/authStore";

const baseUrl = process.env.NEXT_PUBLIC_API_BASE_URL;

const API = axios.create({
  baseURL: baseUrl,
  headers: { "Content-Type": "application/json" },
});

// Attach access token and refresh if about to expire
API.interceptors.request.use(
  async (config) => {
    const { accessToken, refreshToken, setTokens, clearAuth } =
      useAuthStore.getState();

    if (accessToken) {
      // Decode the access token to check its expiry time
      const decodedToken = JSON.parse(atob(accessToken.split(".")[1]));
      const expiryTime = decodedToken.exp * 1000; // Convert to milliseconds
      const currentTime = typeof window !== "undefined" ? Date.now() : 0;

      console.log("Token expiry time:", new Date(expiryTime).toLocaleString());
      console.log("Current time:", new Date(currentTime).toLocaleString());

      // If the token is about to expire (e.g., within 5 minutes), refresh it
      if (expiryTime - currentTime < 5 * 60 * 1000) {
        console.log("Token is about to expire. Refreshing...");

        try {
          const response = await axios.post(`${baseUrl}/Auth/refreshToken`, {
            refreshToken,
          });

          console.log("Refresh token response:", response.data); // Debugging: Log the response

          const { token: newAccessToken } = response.data;

          if (!newAccessToken) {
            throw new Error("Invalid response from refresh token endpoint");
          }

          console.log("New access token:", newAccessToken);

          // Update only the access token, keep the existing refresh token
          setTokens(newAccessToken, refreshToken); // Update tokens in store
          config.headers.Authorization = `Bearer ${newAccessToken}`; // Update request headers
        } catch (refreshError) {
          console.error("Failed to refresh token:", refreshError);
          clearAuth(); // Clear tokens if refresh fails
          if (typeof window !== "undefined") {
            window.location.href = "/login"; // Full reload prevents hydration issues
          } // Redirect to login
        }
      } else {
        config.headers.Authorization = `Bearer ${accessToken}`;
      }
    }

    return config;
  },
  (error) => Promise.reject(error)
);

// Handle expired tokens (401 Unauthorized errors)
API.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    // Check if the error is due to an expired token
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true; // Mark the request as retried

      console.log("Access token expired. Attempting to refresh token...");

      try {
        const { refreshToken, setTokens, clearAuth } = useAuthStore.getState();

        if (!refreshToken) {
          console.error("No refresh token available. Logging out...");
          clearAuth(); // Clear tokens
          if (typeof window !== "undefined") {
            window.location.href = "/login"; // Full reload prevents hydration issues
          }
          throw new Error("No refresh token available");
        }

        // Refresh the access token
        const response = await axios.post(`${baseUrl}/Auth/refreshToken`, {
          refreshToken,
        });

        console.log("Refresh token response:", response.data); // Debugging: Log the response

        const { token: newAccessToken } = response.data;

        if (!newAccessToken) {
          throw new Error("Invalid response from refresh token endpoint");
        }

        console.log("New access token:", newAccessToken);

        // Update only the access token, keep the existing refresh token
        setTokens(newAccessToken, refreshToken); // Update tokens in store

        // Retry the original request with the new access token
        originalRequest.headers.Authorization = `Bearer ${newAccessToken}`;
        return API(originalRequest);
      } catch (refreshError) {
        console.error(
          "Refresh token expired or invalid. Logging out...",
          refreshError
        );
        clearAuth(); // Clear tokens if refresh fails
        if (typeof window !== "undefined") {
          window.location.href = "/login"; // Full reload prevents hydration issues
        }
      }
    }

    return Promise.reject(error);
  }
);

export default API;
