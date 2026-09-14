import React from "react";
import PublicIcon from "@mui/icons-material/Public";
import styles from "./SameCountry.module.css";

const SameCountry = ({ sameCountry }) => {
  return (
    <div className={styles.container}>
      <div className={sameCountry ? styles.same : styles.different}>
        <PublicIcon />
        {sameCountry ? "Same Country" : "Different Countries"}
      </div>
    </div>
  );
};

export default SameCountry;