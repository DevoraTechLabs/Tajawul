"use client";

import { useState } from "react";
import { useRouter } from "next/navigation"; // ✅ Use Next.js router for navigation
import { login } from "@/utils/auth";
import Logo from "@/components/ui/Logo";
import styles from "../login.module.css";
import GoogleIcon from "@mui/icons-material/Google";
import Visibility from "@mui/icons-material/Visibility";
import VisibilityOff from "@mui/icons-material/VisibilityOff";
import ErrorOutlineIcon from "@mui/icons-material/ErrorOutline";
import PlaceIcon from "@mui/icons-material/Place";
import Link from "next/link";
import Image from "next/image";
import Loading from "@/components/ui/Loading"; // Import Loading Component

export default function LoginPage() {
  const router = useRouter(); // ✅ Initialize Next.js router
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");
    setLoading(true);

    const success = await login(email, password);

    setLoading(false);
    if (success) {
      router.push("/"); // ✅ Use Next.js navigation instead of window.location.href
    } else {
      setError("Invalid email or password. Please try again.");
    }
  };

  return (
    <div className={styles.formPageWrapper}>
      <div className={styles.authContainer}>
        {loading && <Loading />} {/* Show Loading Component when loading */}
        <div className={styles.left}>
          <div className={styles.leftContent}>
            <Logo width={120} height={100} />
            <div className={styles.formContainer}>
              <p className={styles.title}>Welcome Back</p>
              <form onSubmit={handleSubmit} className={styles.formWidth}>
                <label>
                  Email Address
                  <input
                    type="email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    required
                    placeholder="Email Address"
                  />
                </label>
                <label className={styles.passwordLabel}>
                  Password
                  <div className={styles.passwordContainer}>
                    <input
                      type={showPassword ? "text" : "password"}
                      value={password}
                      onChange={(e) => setPassword(e.target.value)}
                      required
                      placeholder="Password"
                    />
                    <button
                      type="button"
                      className={styles.togglePassword}
                      onClick={() => setShowPassword(!showPassword)}
                    >
                      {showPassword ? <VisibilityOff /> : <Visibility />}
                    </button>
                  </div>
                </label>
                {error && (
                  <div className={styles.errorMessage}>
                    <ErrorOutlineIcon
                      style={{
                        color: "var(--Error-Text)",
                        fontSize: "20px",
                        marginRight: "8px",
                      }}
                    />
                    {error}
                  </div>
                )}
                <Link href="../forgot-password" className={styles.link}>
                  <div className={styles.forgot}>Forgot Password?</div>
                </Link>
                <button
                  type="submit"
                  className={styles.submitButton}
                  disabled={loading}
                >
                  {loading ? "Logging in..." : "Log In"}
                </button>
              </form>
              <div className={styles.divider}>
                <span>Not a Member?</span>
              </div>
              <p className={styles.registerText}>
                <Link href="../signup" className={styles.link}>
                  Register
                </Link>
                &nbsp;to unlock the best of Tajawul
              </p>
            </div>
          </div>
        </div>
        <div className={styles.right}>
          <div className={styles.locationTag}>
            <PlaceIcon style={{ fontSize: "20px" }} />{" "}
            <p>United Arab Emirates</p>
          </div>
          <Image
            src="/GRAND_MOSQUE.jpg"
            alt="The Grand Mosque in United Arab Emirates"
            fill
            style={{ objectFit: "cover" }}
            priority
          />
        </div>
      </div>
    </div>
  );
}
