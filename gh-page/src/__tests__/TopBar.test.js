import React from "react";
import Enzyme, {shallow} from "enzyme";
import Adapter from "enzyme-adapter-react-16";
import TopBar from "../components/shared/TopBar";
import TopBarLogo from "../components/shared/TopBarLogo";

Enzyme.configure({adapter: new Adapter()});

describe("TopBar", () => {
    it("renders without crashing", () => {
        const wrapper = shallow(<TopBar />);
        expect(wrapper.exists()).toBe(true);
    });
  
    it("matches snapshot", () => {
        const wrapper = shallow(<TopBar />);
        expect(wrapper).toMatchSnapshot();
    });
    
    it("has correct class name", () => {
        const wrapper = shallow(<TopBar />);
        expect(wrapper.hasClass("app-header")).toBe(true);
    });
    
    it("renders TopBarLogo", () => {
        const wrapper = shallow(<TopBar />);
        expect(wrapper.find(<TopBarLogo/>)).toBeDefined();
    });
});