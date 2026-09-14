import NavBar from "@/components/ui/NavBar";
import styles from "@/forms.module.css";

export const metadata = {
  title: "Complete Your Profile",
};

export default function ({ children }) {
  return (
    <div className={styles.container}>
      <NavBar />
      <div className={styles.frameBackground}>
        <img
          src="/arabic-typo-pattern.svg"
          alt="Arabic Typography Pattern"
          className={styles.svgPattern}
        />
        <div className={styles.formFrame}>{children}</div>
      </div>
    </div>
  );
}
