"use client";
import TranslateIcon from "@mui/icons-material/Translate";
import Link from "next/link";

export default function TranslationShortcut() {
  return (
    <div className="resizable-btn-frame">
      <Link href="/translate">
        <button className="resizable-btn">
          <TranslateIcon />
          <span>Translate</span>
        </button>
      </Link>
    </div>
  );
}
