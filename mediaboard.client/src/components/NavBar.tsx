import { Link } from "@tanstack/react-router";
import SearchBar from "@/features/search/SearchBar";

const NavBar = () => {
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
		</nav>
	);
};

export default NavBar;
