"use client";

import styles from "../login.module.css";
import Logo from "@/components/ui/Logo";
import ErrorOutlineIcon from "@mui/icons-material/ErrorOutline";
import MarkEmailReadIcon from "@mui/icons-material/MarkEmailRead";
import PlaceIcon from "@mui/icons-material/Place";
import axios from "axios";
import Link from "next/link";
import Image from "next/image";
import { useState } from "react";

export default function ForgotPassPage() {
  const baseUrl = process.env.NEXT_PUBLIC_API_BASE_URL;
  const [email, setEmail] = useState("");
  const [error, setError] = useState("");
  const [success, setSuccess] = useState(""); // ✅ New state for success message

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");
    setSuccess(""); // ✅ Clear previous messages

    try {
      const response = await axios.post(
        `${baseUrl}/Auth/sendResetPasswordEmail`,
        {
          email,
          clientURI: "https://tajawul.vercel.app/new-password", // ✅ Add client URI
        }
      );

      setSuccess(response.data.message || "Reset link sent successfully!"); // ✅ Store success message
    } catch (err) {
      setError(err.response?.data?.message || "Reset Link Failed");
    }
  };

  return (
    <div className={styles.authContainer}>
      {/* Header: Logo */}
      <div className={styles.left}>
        <div className={styles.leftContent}>
          <Logo width={120} height={100} />
          {/* Form Container */}
          <div className={styles.formContainer}>
            <div>
              <p className={styles.title}>Forgot Your Password?</p>
              <p className={styles.subtitle}>
                Enter your email address to reset your password
              </p>
            </div>

            {/* Start Form Items */}
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

              {/* ✅ Success message display */}
              {success && (
                <div className={styles.successMessage}>
                  <MarkEmailReadIcon
                    style={{
                      color: "#0d5f07",
                      fontSize: "20px",
                      marginRight: "8px",
                    }}
                  />
                  {success}
                </div>
              )}

              {/* Error message display */}
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

              <button type="submit" className={styles.submitButton}>
                Send Reset Link
              </button>
            </form>

            <p className={styles.registerText}>
              Return to the&nbsp;
              <Link href="../login" className={styles.link}>
                Login Page
              </Link>
            </p>
          </div>
        </div>
      </div>

      <div className={styles.right}>
        <div className={styles.locationTag}>
          <PlaceIcon
            style={{
              fontSize: "20px",
            }}
          />
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
  );
}
