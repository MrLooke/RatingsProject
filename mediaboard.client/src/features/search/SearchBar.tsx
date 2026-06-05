import InputText from "@/components/InputText";

const SearchBar = () => {
    return (
        <InputText
            style={{ width: "350px", marginLeft: "25px", marginRight: "5px" }}
            placeholder="Search..."
        />
    );
};

export default SearchBar;
