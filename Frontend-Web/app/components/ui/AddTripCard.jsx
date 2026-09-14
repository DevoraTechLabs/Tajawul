"use client";

import Link from "next/link";
import styles from "@/components/ui/DestinationCard.module.css";

export default function AddTripCard() {
  return (
    <div className={styles.card}>
      <div className={styles.imageWrapper}>
        <div
          className={styles.image}
          style={{
            backgroundColor: "var(--Blue-Very-Bright)",
            border: "1px dashed var(--Blue-Perfect)",
            display: "flex",
            flexDirection: "column",
            justifyContent: "center",
            alignItems: "center",
            padding: "24px",
            textAlign: "center",
          }}
        >
          <p
            style={{
              color: "var(--Neutral-Black-Text)",
              fontFamily: "Merriweather",
              fontSize: "22px",
              fontWeight: "700",
              marginBottom: "12px",
            }}
          >
            Ready for your next adventure?
          </p>

          <p
            style={{
              color: "var(--Neutral-Black-Text)",
              fontFamily: "DM Sans",
              fontSize: "16px",
              fontWeight: "500",
              marginBottom: "24px",
            }}
          >
            Create and share your trip plan to inspire fellow travelers!
          </p>

          <Link href="/create-trip" className={styles.addDestinationButton}>
            Plan a Trip
          </Link>
        </div>
      </div>
    </div>
  );
}