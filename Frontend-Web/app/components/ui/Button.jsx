import styles from "./Button.module.css"; // Import styles

const Button = ({ type = "primary", size = "14px", children }) => {
  return (
    <button className={`${styles.button} ${styles[type]} ${styles[size]}`}>
      {children}
    </button>
  );
};

export default Button;
