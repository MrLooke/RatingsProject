import { createContext, useContext, useState, useEffect } from "react";
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
	const [user, setUser] = useState<AuthResult | null>(null);

	const { data, isLoading } = useQuery({
		queryKey: ["me"],
		queryFn: checkUser,
		retry: false,
		staleTime: Infinity,
	});

	useEffect(() => {
		if (data) setUser(data);
	}, [data]);

	const login = (user: AuthResult) => setUser(user);

	const queryClient = useQueryClient();
	const logout = () => {
		setUser(null);
		queryClient.removeQueries({ queryKey: ["me"] });
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
