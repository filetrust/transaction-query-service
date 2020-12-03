import Enzyme from "enzyme";
import Adapter from "enzyme-adapter-react-16";
import Base64Reader from "../components/swagger/Base64Reader";

Enzyme.configure({adapter: new Adapter()});

describe("Base64Reader.ReadFromInput", () => {
    it("is defined", () => {
        expect(Base64Reader.ReadFromInput).toBeDefined();
    });

    it("returns empty string when input is undefined", async () => {
        const actual = await Base64Reader.ReadFromInput(undefined);
  
        expect(actual).toBe("");
    });
  
    it("returns empty string when input is null", async () => {
        const actual = await Base64Reader.ReadFromInput(null);
  
        expect(actual).toBe("");
    });
  
    it("returns empty string when input has no files", async () => {
        const input = document.createElement("input");
        const actual = await Base64Reader.ReadFromInput(input);
  
        expect(actual).toBe("");
    });
});