import React from "react";
import Enzyme, {shallow} from "enzyme";
import Adapter from "enzyme-adapter-react-16";
import TopBarLogo from "../components/shared/TopBarLogo";
import logo from "../img/logo.svg";

Enzyme.configure({adapter: new Adapter()});

describe("TopBarLogo", () => {
    it("renders without crashing", () => {
        const wrapper = shallow(<TopBarLogo />);
        expect(wrapper.exists()).toBe(true);
    });
  
    it("matches snapshot", () => {
        const wrapper = shallow(<TopBarLogo />);
        expect(wrapper).toMatchSnapshot();
    });
    
    it("base classname matches", () => {
        const wrapper = shallow(<TopBarLogo />);
        expect(wrapper.hasClass("logo")).toBe(true);
    });
      
    it("renders img", () => {
        const wrapper = shallow(<TopBarLogo />);
        expect(wrapper.find("img")).toBeDefined();
    });
    
    it("renders img with src", () => {
        const wrapper = shallow(<TopBarLogo />);
        expect(wrapper.find("img").prop("src")).toBe(logo);
    });
    
    it("renders img with alt", () => {
        const wrapper = shallow(<TopBarLogo />);
        expect(wrapper.find("img").prop("alt")).toBe("Logo");
    });
    
    it("renders img with height", () => {
        const wrapper = shallow(<TopBarLogo />);
        expect(wrapper.find("img").prop("height")).toBe("90");
    });
});