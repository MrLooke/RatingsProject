import { useEffect, useRef, useState } from "react";
import { Link, useNavigate } from "@tanstack/react-router";
import SearchBar from "@/features/search/SearchBar";
import ProfileImage from "@/components/ProfileImage";
import { useAuth } from "@/context/AuthContext";
import { logoutUser } from "@/api/authApi";
import styles from "./NavBar.module.css";

const NavBar = () => {
	const { user, logout } = useAuth();
	const navigate = useNavigate();
	const [open, setOpen] = useState(false);
	const dropdownRef = useRef<HTMLDivElement>(null);

	useEffect(() => {
		const handler = (e: MouseEvent) => {
			if (
				dropdownRef.current &&
				!dropdownRef.current.contains(e.target as Node)
			) {
				setOpen(false);
			}
		};
		document.addEventListener("mousedown", handler);
		return () => document.removeEventListener("mousedown", handler);
	}, []);

	const handleLogout = () => {
		logoutUser();
		logout();
		setOpen(false);
		navigate({ to: "/" });
	};

	return (
		<nav>
			<h3>MusicBoard</h3>
			<SearchBar />
			<ul>
				<Link to="/">Home</Link>
			</ul>
			<ul>
				<Link to="/about">About</Link>
			</ul>
			<div className={styles.profileSection} ref={dropdownRef}>
				{user && (
					<span className={styles.username}>{user.username}</span>
				)}
				<button
					className={styles.profileButton}
					onClick={() => setOpen((prev) => !prev)}
					aria-label="Profile menu"
				>
					<ProfileImage className={styles.profileIcon} />
				</button>
				{open && (
					<div className={styles.dropdown}>
						{!user ? (
							<>
								<Link
									to="/login"
									className={styles.dropdownItem}
									onClick={() => setOpen(false)}
								>
									Log In
								</Link>
								<Link
									to="/register"
									className={styles.dropdownItem}
									onClick={() => setOpen(false)}
								>
									Create Account
								</Link>
							</>
						) : (
							<>
								<button
									className={styles.dropdownItem}
									disabled
								>
									Settings
								</button>
								<button
									className={styles.dropdownItem}
									onClick={handleLogout}
								>
									Log Out
								</button>
							</>
						)}
					</div>
				)}
			</div>
		</nav>
	);
};

export default NavBar;
