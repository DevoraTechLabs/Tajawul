import styles from "@/styles/Chatbot.module.css";

export default function ChatLoading() {
  return (
    <div className={styles.loadingWrapper}>
      <div className={styles.loadingDot} />
      <div className={styles.loadingDot} />
      <div className={styles.loadingDot} />
    </div>
  );
}
