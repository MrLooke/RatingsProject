import { createContext, useContext, useState } from "react";
import type { ReactNode } from "react";
import type { AuthResult } from "@/api/authApi";

interface AuthContextType {
	user: AuthResult | null;
	login: (user: AuthResult) => void;
	logout: () => void;
}

const AuthContext = createContext<AuthContextType | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
	const [user, setUser] = useState<AuthResult | null>(null);

	const login = (user: AuthResult) => setUser(user);
	const logout = () => setUser(null);

	return (
		<AuthContext.Provider value={{ user, login, logout }}>
			{children}
		</AuthContext.Provider>
	);
}

export function useAuth() {
	const ctx = useContext(AuthContext);
	if (!ctx) throw new Error("useAuth must be used within AuthProvider");
	return ctx;
}
