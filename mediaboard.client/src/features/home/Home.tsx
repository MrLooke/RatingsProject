import "@/App.css";
import Button from "@/components/Button.tsx";

function Home() {
    return (
        <div>
            <div className="card">
                <h1>Sharp Heading</h1>
                <p>A muted shade for the rest of the text.</p>
                <Button variant="primary">Some Action</Button>
            </div>
        </div>
    );
}

export default Home;
