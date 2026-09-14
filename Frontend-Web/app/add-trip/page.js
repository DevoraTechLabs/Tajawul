"use client";

import NavBar from "@/components/ui/NavBar";
import styles from "@/forms.module.css";
import CreateTripForm from "./CreateTripForm";
import withAuth from "@/utils/withAuth";

function AddTrip() {
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
          <p className={styles.title}>Create New Trip</p>
          <CreateTripForm />
        </div>
      </div>
    </div>
  );
}

export default withAuth(AddTrip);