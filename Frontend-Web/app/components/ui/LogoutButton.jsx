"use client";

import { useState } from "react";
import { logout } from "@/utils/auth";
import styles from "./NavBar.module.css";

export default function LogoutButton() {
  const [loading, setLoading] = useState(false);

  const handleLogout = async () => {
    setLoading(true);
    await logout(); // ✅ Calls the logout function
    setLoading(false);
  };

  return (
    <button
      onClick={handleLogout}
      disabled={loading}
      className={styles.dropdownItem}
    >
      {loading ? "Logging out..." : "Logout"}
    </button>
  );
}
