import "./globals.css";
import { AnimatePresence } from "framer-motion";
import { SpeedInsights } from "@vercel/speed-insights/next";
import { dmSans, merriweather } from "./fonts";

export const metadata = {
  title: "Tajawul",
  description: "Explore the heart of the Arab World",
};

export default function RootLayout({ children }) {
  return (
    <html lang="en" className={`${dmSans.variable} ${merriweather.variable}`}>
      <body>
        <AnimatePresence mode="wait">{children}</AnimatePresence>
        <SpeedInsights />
      </body>
    </html>
  );
}
