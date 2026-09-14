"use client";

import Link from "next/link";
import styles from "@/components/ui/DestinationCard.module.css";

export default function AddDestinationCard() {
  return (
    <div className={styles.card}>
      <div className={styles.imageWrapper}>
        <div
          className={styles.image}
          style={{
            backgroundColor: "var(--Green-Very-Bright)",
            border: "1px dashed var(--Green-Perfect)",
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
            Discovered a not listed hidden gem?
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
            Add it now so other travelers don’t miss out on something special!
          </p>

          <Link href="/add-destination" className={styles.addDestinationButton}>
            Add Destination
          </Link>
        </div>
      </div>
    </div>
  );
}
