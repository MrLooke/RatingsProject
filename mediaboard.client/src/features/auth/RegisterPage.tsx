import { useState } from "react";
import { Link, useNavigate } from "@tanstack/react-router";
import { registerUser } from "@/api/authApi";
import { useAuth } from "@/context/AuthContext";
import Button from "@/components/Button";
import InputText from "@/components/InputText";
import styles from "./auth.module.css";

function RegisterPage() {
    const { login } = useAuth();
    const navigate = useNavigate();
    const [username, setUsername] = useState("");
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState<string | null>(null);
    const [loading, setLoading] = useState(false);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError(null);
        setLoading(true);
        try {
            const result = await registerUser({ username, email, password });
            login(result);
            navigate({ to: "/" });
        } catch (err) {
            setError(err instanceof Error ? err.message : "Registration failed.");
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className={styles.authContainer}>
            <div className={styles.authCard}>
                <h2>Create Account</h2>
                <form onSubmit={handleSubmit} className={styles.form}>
                    <div className={styles.field}>
                        <label htmlFor="username">Username</label>
                        <InputText
                            id="username"
                            type="text"
                            value={username}
                            onChange={(e) => setUsername(e.target.value)}
                            placeholder="Username"
                            className={styles.input}
                            required
                        />
                    </div>
                    <div className={styles.field}>
                        <label htmlFor="email">Email</label>
                        <InputText
                            id="email"
                            type="email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            placeholder="your@email.com"
                            className={styles.input}
                            required
                        />
                    </div>
                    <div className={styles.field}>
                        <label htmlFor="password">Password</label>
                        <InputText
                            id="password"
                            type="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            placeholder="Min. 8 characters"
                            className={styles.input}
                            required
                        />
                    </div>
                    {error && <p className={styles.error}>{error}</p>}
                    <Button type="submit" disabled={loading} size="large">
                        {loading ? "Creating account..." : "Create Account"}
                    </Button>
                </form>
                <p className={styles.switchPrompt}>
                    Already have an account? <Link to="/login">Log in</Link>
                </p>
            </div>
        </div>
    );
}

export default RegisterPage;
