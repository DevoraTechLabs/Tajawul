"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import useAuthStore from "@/store/authStore";

export default function withAuth(Component) {
  return function ProtectedComponent(props) {
    const router = useRouter();
    const { accessToken } = useAuthStore();
    const [loading, setLoading] = useState(true);

    useEffect(() => {
      if (!accessToken) {
        router.push("/login"); // ✅ Redirect if not logged in
      } else {
        setLoading(false); // ✅ Show content if authenticated
      }
    }, [accessToken, router]);

    if (loading) return <p>Loading...</p>; // ✅ Show a loading state

    return <Component {...props} />;
  };
}
