import { create } from "zustand";

const useAuthStore = create((set) => ({
  accessToken:
    typeof window !== "undefined" ? localStorage.getItem("authToken") : null,
  refreshToken:
    typeof window !== "undefined" ? localStorage.getItem("refreshToken") : null,
  roles:
    typeof window !== "undefined"
      ? JSON.parse(localStorage.getItem("roles") || "[]")
      : [],
  profileData:
    typeof window !== "undefined"
      ? JSON.parse(localStorage.getItem("profileData") || null)
      : null,

  setAuth: (accessToken, refreshToken, roles, profileData) => {
    localStorage.setItem("authToken", accessToken);
    localStorage.setItem("refreshToken", refreshToken);
    localStorage.setItem("roles", JSON.stringify(roles));
    if (profileData) {
      localStorage.setItem("profileData", JSON.stringify(profileData));
    }
    set({ accessToken, refreshToken, roles, profileData });
  },

  setTokens: (accessToken, refreshToken) => {
    localStorage.setItem("authToken", accessToken);
    localStorage.setItem("refreshToken", refreshToken);
    set({ accessToken, refreshToken });
  },

  setRoles: (roles) => {
    localStorage.setItem("roles", JSON.stringify(roles));
    set({ roles });
  },

  addRole: (role) => {
    set((state) => {
      const updatedRoles = [...new Set([...state.roles, role])];
      localStorage.setItem("roles", JSON.stringify(updatedRoles));
      return { roles: updatedRoles };
    });
  },

  replaceRoles: (newRoles) => {
    localStorage.setItem("roles", JSON.stringify(newRoles));
    set({ roles: newRoles });
  },

  replaceTokens: (accessToken, refreshToken) => {
    localStorage.setItem("authToken", accessToken);
    localStorage.setItem("refreshToken", refreshToken);
    set({ accessToken, refreshToken });
  },

  removeRole: (role) => {
    set((state) => {
      const updatedRoles = state.roles.filter((r) => r !== role);
      localStorage.setItem("roles", JSON.stringify(updatedRoles));
      return { roles: updatedRoles };
    });
  },

  hasRole: (role) => {
    return useAuthStore.getState().roles.includes(role);
  },

  setProfileData: (profileData) => {
    localStorage.setItem("profileData", JSON.stringify(profileData));
    set({ profileData });
  },

  clearAuth: () => {
    localStorage.removeItem("authToken");
    localStorage.removeItem("refreshToken");
    localStorage.removeItem("roles");
    localStorage.removeItem("profileData");
    set({
      accessToken: null,
      refreshToken: null,
      roles: [],
      profileData: null,
    });
  },
}));

export default useAuthStore;
