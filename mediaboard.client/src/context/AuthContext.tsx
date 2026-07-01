import { createContext, useContext } from "react";
import { useQueryClient, useQuery } from "@tanstack/react-query";
import type { ReactNode } from "react";
import type { AuthResult } from "@/api/authApi";
import { checkUser } from "@/api/authApi";

interface AuthContextType {
	user: AuthResult | null;
	isLoading: boolean;
	login: (user: AuthResult) => void;
	logout: () => void;
}

const AuthContext = createContext<AuthContextType | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
	const queryClient = useQueryClient();

	const { data: user = null, isLoading } = useQuery({
		queryKey: ["me"],
		queryFn: checkUser,
		retry: false,
		staleTime: Infinity,
	});

	const login = (userData: AuthResult) => {
		queryClient.setQueryData(["me"], userData);
	};

	const logout = () => {
		queryClient.setQueryData(["me"], null);
	};

	return (
		<AuthContext.Provider value={{ user, isLoading, login, logout }}>
			{children}
		</AuthContext.Provider>
	);
}

export function useAuth() {
	const ctx = useContext(AuthContext);
	if (!ctx) throw new Error("useAuth must be used within AuthProvider");
	return ctx;
}
