"use client";

import NavBar from "@/components/ui/NavBar";
import styles from "@/forms.module.css";
import UploadDestinationImages from "./UploadDestinationImages";
import withAuth from "@/utils/withAuth"; // ✅ Import the HOC

function UploadImages() {
  return (
    <div className={styles.container}>
      <NavBar />
      <div className={styles.frameBackground}>
        <img
          src="/arabic-typo-pattern.svg"
          alt="Arabic Typography Pattern"
          className={styles.svgPattern}
        />
        <div className={styles.formFrame}>
          <h1 className={styles.smallTitle}>Add Images for Your Destination</h1>
          <div className={styles.noteContainer}>
            <p className={styles.note}>
              <strong>Accepted formats:</strong> JPG, JPEG, PNG, WEBP
            </p>
            <p className={styles.note}>
              <strong> Maximum file size:</strong> 2MB
            </p>
          </div>

          <UploadDestinationImages />
        </div>
      </div>
    </div>
  );
}

export default withAuth(UploadImages); // ✅ Protect the page
