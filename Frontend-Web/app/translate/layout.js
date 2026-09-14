import NavBar from "@/components/ui/NavBar";

export const metadata = {
  title: "Tajawul Translate",
};

export default function Layout({ children }) {
  return (
    <div>
      <NavBar /> {children}
    </div>
  );
}
