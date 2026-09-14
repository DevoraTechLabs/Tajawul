"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import useAuthStore from "@/store/authStore";
import Logo from "./Logo";
import TranslationShortcut from "./TranslationShortcut";
import CircleNotificationsIcon from "@mui/icons-material/CircleNotifications";
import KeyboardArrowDownIcon from "@mui/icons-material/KeyboardArrowDown";
import Avatar from "@mui/material/Avatar";
import Button from "./Button";
import styles from "./NavBar.module.css";
import LogoutButton from "@/components/ui/LogoutButton";
import { useEffect, useState, Suspense } from "react";
import API from "@/utils/api";

export default function NavBar() {
  const { accessToken, profileData, setProfileData } = useAuthStore();
  const pathname = usePathname();
  const [isMounted, setIsMounted] = useState(false);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    setIsMounted(true);

    // Only fetch if we have accessToken but no profileData in the store
    if (accessToken && !profileData) {
      fetchProfileData();
    }
  }, [accessToken]); // Removed profileData from dependencies

  const fetchProfileData = async () => {
    try {
      setLoading(true);
      const response = await API.get("/User/profile");
      setProfileData(response.data); // This updates the authStore
    } catch (error) {
      console.error("Failed to fetch profile data:", error);
    } finally {
      setLoading(false);
    }
  };

  const getAvatarContent = () => {
    // Show default avatar while loading (only if we have accessToken but no profileData yet)
    if (loading && accessToken && !profileData) {
      return (
        <Avatar
          sx={{
            width: 40,
            height: 40,
            bgcolor: "var(--Neutrals-Black-Text)",
            color: "var(--Beige-Perfect)",
          }}
        />
      );
    }

    if (profileData?.profileImage) {
      return (
        <Avatar
          sx={{
            width: 40,
            height: 40,
            bgcolor: "var(--Neutrals-Black-Text)",
            color: "var(--Beige-Perfect)",
          }}
          src={profileData.profileImage}
          alt="Profile"
        />
      );
    }

    // Use initials if no profile image
    const firstNameInitial = profileData?.firstName?.[0] || "";
    const lastNameInitial = profileData?.lastName?.[0] || "";
    return (
      <Avatar
        sx={{
          width: 40,
          height: 40,
          bgcolor: "var(--Neutrals-Black-Text)",
          color: "var(--Beige-Perfect)",
        }}
      >
        {`${firstNameInitial}${lastNameInitial}`}
      </Avatar>
    );
  };

  // Don't render anything until mounted (client-side)
  if (!isMounted) return null;

  return (
    <nav className={styles.navBar}>
      <div className={styles.logoTransFrame}>
        <Link
          className={`${styles.homeItem} ${
            pathname === "/" ? styles.active : ""
          }`}
          href="/"
          aria-label="Home"
        >
          <Logo />
        </Link>
        <TranslationShortcut />
      </div>
      <Link
        className={`${styles.navItem} ${
          pathname === "/explore" ? styles.active : ""
        }`}
        href="/explore"
        aria-label="Explore"
      >
        Explore
      </Link>

      {/* Trips Dropdown */}
      <div className={styles.dropdownContainer}>
        <button className={styles.dropItem} aria-label="Trips">
          Trips{" "}
          <KeyboardArrowDownIcon
            className={styles.arrowIcon}
            sx={{ fontSize: 18 }}
          />
        </button>
        <div className={styles.dropdownMenu}>
          <Link href="/triphub" className={styles.dropdownItem}>
            TripHub
          </Link>
          <Link href="/add-trip" className={styles.dropdownItem}>
            Create New Trip
          </Link>
        </div>
      </div>

      <Link
        className={`${styles.navItem} ${
          pathname === "/connect" ? styles.active : ""
        }`}
        href="/connect"
        aria-label="Connect"
      >
        Connect
      </Link>
      <Link
        className={`${styles.navItem} ${
          pathname === "/about" ? styles.active : ""
        }`}
        href="/about"
        aria-label="About"
      >
        About
      </Link>

      {/* Show Login/Register buttons if NOT logged in */}
      {!accessToken && (
        <div className={styles.loginFrame}>
          <Link href="/login">
            <Button type="secondary" size="px14">
              Login
            </Button>
          </Link>
          <Link href="/signup">
            <Button type="primary" size="px14">
              Register
            </Button>
          </Link>
        </div>
      )}

      {/* Show Profile & Avatar Dropdown if user IS logged in */}
      {accessToken && (
        <div className={styles.profileFrame}>
          <CircleNotificationsIcon
            sx={{ width: 48, height: 48, color: "var(--Neutrals-Black-Text)" }}
            aria-label="Notifications"
          />
          {/* Avatar Dropdown */}
          <div className={styles.dropdownContainer}>
            <button className={styles.avatarButton} aria-label="Profile">
              <Suspense
                fallback={
                  <Avatar
                    sx={{
                      width: 40,
                      height: 40,
                      bgcolor: "var(--Neutrals-Black-Text)",
                      color: "var(--Beige-Perfect)",
                    }}
                  />
                }
              >
                {getAvatarContent()}
              </Suspense>
            </button>
            <div className={styles.dropdownMenuAvatar}>
              <Link href="/profile" className={styles.dropdownItem}>
                Profile
              </Link>

              <LogoutButton className={styles.dropdownItem} />
            </div>
          </div>
        </div>
      )}
    </nav>
  );
}
