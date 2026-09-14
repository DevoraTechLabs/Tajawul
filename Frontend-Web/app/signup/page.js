"use client";

import styles from "../login.module.css";
import Logo from "@/components/ui/Logo";
import Visibility from "@mui/icons-material/Visibility";
import VisibilityOff from "@mui/icons-material/VisibilityOff";
import ErrorOutlineIcon from "@mui/icons-material/ErrorOutline";
import PlaceIcon from "@mui/icons-material/Place";
import Loading from "@/components/ui/Loading"; // Adjust the path if needed
import axios from "axios";
import Link from "next/link";
import Image from "next/image";
import { useState } from "react";

export default function SignUpPage() {
  const baseUrl = process.env.NEXT_PUBLIC_API_BASE_URL;
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);
  const [showPopup, setShowPopup] = useState(false);
  const [resendLoading, setResendLoading] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (loading) return;

    setError("");
    setLoading(true);

    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(email)) {
      setError("Invalid email format.");
      setLoading(false);
      return;
    }

    // Password validation regex:
    const passwordRegex =
      /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()\-_=\+\\|[\]{};:/?.><])[A-Za-z\d!@#$%^&*()\-_=\+\\|[\]{};:/?.><]{8,}$/;

    if (!passwordRegex.test(password)) {
      setError(
        "Password must be at least 8 characters long, contain at least one uppercase letter, one lowercase letter, one number, and one special character."
      );
      setLoading(false);
      return;
    }

    if (password.length < 8) {
      setError("Password must be at least 8 characters long.");
      setLoading(false);
      return;
    }

    if (password !== confirmPassword) {
      setError("Passwords do not match.");
      setLoading(false);
      return;
    }

    try {
      await axios.post(`${baseUrl}/Auth/signup`, {
        email: email,
        password,
        confirmPassword,
        clientURI: "https://tajawul.vercel.app/email-verification",
      });

      setShowPopup(true);
    } catch (err) {
      setError(err.response?.data?.message || "Signup failed");
    } finally {
      setLoading(false);
    }
  };

  const handleResendEmail = async () => {
    if (resendLoading) return; // Prevent multiple clicks
    if (!email) {
      alert("Email is missing. Please try again.");
      return;
    }

    setResendLoading(true);
    try {
      const response = await axios.post(
        `${baseUrl}/Auth/sendEmailVerification`,
        {
          email,
          clientURI: "https://tajawul.vercel.app/email-verification",
        }
      );
      alert("A new confirmation email has been sent! 📩");
    } catch (error) {
      alert(
        `Failed to resend email: ${
          error.response?.data?.message || "Unknown error"
        }`
      );
    } finally {
      setResendLoading(false);
    }
  };

  return (
    <div className={styles.authContainer}>
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
      {/* Header: Logo */}
      <div className={styles.left}>
        <div className={styles.leftContent}>
          <Logo width={120} height={100} />
          {/* Form Container */}
          <div className={styles.formContainer}>
            <div>
              <p className={styles.title}>Create New Account</p>
              <p className={styles.subtitle}>
                Join us to unlock travel inspiration, personalized
                Recommendations, and connections with fellow explorers!
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
              {/* Confirm Password */}
              <label className={styles.passwordLabel}>
                Confirm Password
                <div className={styles.passwordContainer}>
                  <input
                    type={showConfirmPassword ? "text" : "password"}
                    value={confirmPassword}
                    onChange={(e) => setConfirmPassword(e.target.value)}
                    required
                    placeholder="Confirm Password"
                  />
                  <button
                    type="button"
                    className={styles.togglePassword}
                    onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                  >
                    {showConfirmPassword ? <VisibilityOff /> : <Visibility />}
                  </button>
                </div>
              </label>

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
                Register Now
              </button>
            </form>

            <div className={styles.divider}>
              <span>Already a member?</span>
            </div>
            <p className={styles.registerText}>
              <Link href="../login" className={styles.link}>
                Log In
              </Link>
              &nbsp;using your Tajawul account
            </p>
          </div>
        </div>
      </div>
      {loading && <Loading />}
      {showPopup && (
        <div className={styles.popupOverlay}>
          <div className={styles.popupContent}>
            <Image
              src="/email-sent.svg"
              alt="An email notification illustration"
              width={100}
              height={120}
              priority
            />
            <button
              className={styles.closeButton}
              onClick={() => setShowPopup(false)}
            >
              ✕
            </button>

            <h2>Email Confirmation</h2>
            <p>
              We have sent an email to <strong>{email}</strong> to confirm the
              validity of your email address. After receiving the email, follow
              the link provided to complete your registration.
            </p>
            <div className={styles.divider}>Or</div>
            <p>
              If you did not receive any mail,{" "}
              <button
                className={styles.resendButton}
                onClick={handleResendEmail}
                disabled={resendLoading}
              >
                Resend confirmation mail
              </button>
            </p>
          </div>
        </div>
      )}
    </div>
  );
}
