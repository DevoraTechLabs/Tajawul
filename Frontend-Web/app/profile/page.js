"use client";

import withAuth from "@/utils/withAuth"; // ✅ Import the HOC
import SearchBar from "@/components/ui/SearchBar";

function ProfilePage() {
  return (
    <div className="container">
      <h1>Profile</h1>
      <SearchBar searchType="destination" />
    </div>
  );
}

export default withAuth(ProfilePage); // ✅ Protect the page
