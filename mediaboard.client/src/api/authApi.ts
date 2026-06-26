const apiUrl = import.meta.env.VITE_API_URL;

export interface AuthResult {
    userId: number;
    username: string;
    email: string;
}

export interface RegisterRequest {
    username: string;
    email: string;
    password: string;
}

export interface LoginRequest {
    email: string;
    password: string;
}

export const registerUser = async (request: RegisterRequest): Promise<AuthResult> => {
    const response = await fetch(`${apiUrl}/auth/register`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(request),
    });

    if (!response.ok) {
        const error = await response.json();
        throw new Error(error.error ?? "Registration failed.");
    }

    return response.json();
};

export const loginUser = async (request: LoginRequest): Promise<AuthResult> => {
    const response = await fetch(`${apiUrl}/auth/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(request),
    });

    if (!response.ok) {
        const error = await response.json();
        throw new Error(error.error ?? "Login failed.");
    }

    return response.json();
};
