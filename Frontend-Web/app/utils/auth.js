import axios from "axios";
import useAuthStore from "@/store/authStore";
import API from "./api"; // Import the configured API instance

export async function login(email, password) {
  const baseUrl = process.env.NEXT_PUBLIC_API_BASE_URL;

  try {
    if (!baseUrl) {
      throw new Error("API base URL is not defined in environment variables.");
    }

    const response = await axios.post(`${baseUrl}/Auth/signin`, {
      email,
      password,
    });

    // Extract tokens and roles
    const { token, refreshToken, role } = response.data;

    if (!token || !refreshToken) {
      throw new Error("Invalid response from server - missing tokens");
    }

    // Convert role to array if it's a string (backward compatibility)
    const roles = Array.isArray(role) ? role : [role].filter(Boolean);

    // Debug logging
    console.log("Login successful. Roles:", roles);

    // Store access token, refresh token, and roles
    useAuthStore.getState().setAuth(token, refreshToken, roles);

    return true; // ✅ Login successful
  } catch (error) {
    console.error(
      "Login Failed:",
      error.response?.data?.message || error.message
    );
    return false; // ❌ Login failed
  }
}

export async function logout() {
  try {
    // Correct way to access clearAuth
    const { clearAuth } = useAuthStore.getState();

    await API.post("/Auth/logout");

    // Now this will work
    clearAuth();

    if (typeof window !== "undefined") {
      window.location.href = "/login";
    }
  } catch (error) {
    console.error("Logout failed:", error);
    // Fallback cleanup
    useAuthStore.getState().clearAuth();
    if (typeof window !== "undefined") {
      window.location.href = "/login";
    }
  }
}
