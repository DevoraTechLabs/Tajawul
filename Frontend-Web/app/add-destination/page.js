"use client";

import NavBar from "@/components/ui/NavBar";
import styles from "@/forms.module.css";
import CreateDestinationForm from "./CreateDestinationForm";
import withAuth from "@/utils/withAuth"; // ✅ Import the HOC

function AddDestination() {
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
          <CreateDestinationForm />
        </div>
      </div>
    </div>
  );
}

export default withAuth(AddDestination); // ✅ Protect the page
