"use client";

import Image from "next/image";
import Link from "next/link";
import NavBar from "./components/ui/NavBar";
import styles from "@/home.module.css";
import { useEffect, useState } from "react";
import useAuthStore from "@/store/authStore";
import withAuth from "@/utils/withAuth"; // Import the withAuth HOC
import SearchBar from "@/components/ui/SearchBar";
import FloatingChatButton from "@/components/ui/FloatingChatButton"; // Import the floating chat button component
import RecommendedDest from "@/components/ui/Recommendation/RecommendedDest";
import TopRatingDest from "@/components/ui/Recommendation/TopRatingDest";
import CurrencyConverter from "@/currency-converter/CurrencyConverter"; // Import the currency converter component
import Weather from "@/currency-converter/Weather";

function Home() {
  const [showPopup, setShowPopup] = useState(false);
  const [popupType, setPopupType] = useState("initial");
  const { roles } = useAuthStore();

  useEffect(() => {
    // Check if user is logged in and has "Person" role
    if (roles.includes("Person")) {
      if (
        roles.includes("CompletedInterestInfo") ||
        roles.includes("CompletedSocialInfo")
      ) {
        setPopupType("partial");
      } else {
        setPopupType("initial");
      }
      setShowPopup(true);
    }
  }, [roles]);

  const handleClosePopup = () => {
    setShowPopup(false);
  };

  return (
    <div>
      <NavBar />
      <CurrencyConverter />
      <Weather />
      <div className={styles.container}>
        <FloatingChatButton />
        <main>
          {/* Hero Section */}
          <section className={styles.heroSection}>
            <div className={styles.heroBackground}>
              <Image
                src="/Egypt.jpg"
                alt="Luxor, Egypt"
                fill={true}
                style={{ objectFit: "cover" }}
                priority
              />
            </div>
            <div className={styles.heroContent}>
              <h1>Explore the Heart of the Arab World</h1>
              <p>
                Find unique experiences, hidden gems, and cultural wonders.
                Begin your search!
              </p>
              <SearchBar searchType="all" size="large" />
            </div>
          </section>

          {/* Content Section */}
          <section className={styles.contentSection}>
            <RecommendedDest />
            <TopRatingDest />
          </section>
        </main>
      </div>

      {showPopup && (
        <div className={styles.popupOverlay}>
          <div className={styles.popupContent}>
            {popupType === "initial" ? (
              <>
                <Image
                  src="/welcome-illustration.svg"
                  alt="welcome illustration"
                  width={300}
                  height={300}
                  priority
                />
                <h2>Welcome to Tajawul!</h2>
                <p>
                  Tell us a little about yourself so we can match you with the
                  perfect destinations and fellow travelers. It only takes a
                  couple of minutes — and it makes all the difference.
                </p>
              </>
            ) : (
              <>
                <Image
                  src="/complete-your-profile.svg"
                  alt="complete your profile"
                  width={300}
                  height={300}
                  priority
                />
                <h2>
                  You're one step away from unlocking the full Tajawul
                  experience!
                </h2>
                <p>
                  Complete your profile to access all features — personalized
                  recommendations, social connections, and a travel experience
                  made just for you.
                </p>
              </>
            )}

            <button className={styles.closeButton} onClick={handleClosePopup}>
              ✕
            </button>

            <div className={styles.buttonGroup}>
              <button
                className={styles.secondaryButton}
                onClick={handleClosePopup}
              >
                {popupType === "initial" ? "Maybe Later" : "Remind Me Later"}
              </button>
              <Link
                href="/complete-your-profile/"
                className={styles.submitButton}
              >
                {popupType === "initial"
                  ? "Complete My Profile"
                  : "Continue My Profile"}
              </Link>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

export default withAuth(Home); // Wrap the Home component with withAuth
