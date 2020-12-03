import React from "react";
import Enzyme, {shallow} from "enzyme";
import Adapter from "enzyme-adapter-react-16";
import SwaggerPage from "../components/swagger/SwaggerPage";
import TopBar from "../components/shared/TopBar";
import yam from "../components/swagger/api.yaml";
import Base64Reader from "../components/swagger/Base64Reader";

Enzyme.configure({adapter: new Adapter()});

describe("SwaggerPage", () => {
    it("renders without crashing", () => {
        const wrapper = shallow(<SwaggerPage />);
        expect(wrapper.exists()).toBe(true);
    });
  
    it("matches snapshot", () => {
        const wrapper = shallow(<SwaggerPage />);
        expect(wrapper).toMatchSnapshot();
    });
    
    it("renders topbar", () => {
        const wrapper = shallow(<SwaggerPage />);
        expect(wrapper.contains(<TopBar/>));
    });
  
    it("renders swagger", () => {
        const wrapper = shallow(<SwaggerPage />);
        expect(wrapper.find("SwaggerUI")).toBeDefined();
    });
    
    it("renders swagger with correct yaml", () => {
        const wrapper = shallow(<SwaggerPage />);
        expect(wrapper.find("SwaggerUI").prop("url")).toBe(window.location.origin + yam)
    });
    
    it("renders swagger with docexpansion equalto list", () => {
        const wrapper = shallow(<SwaggerPage />);
        expect(wrapper.find("SwaggerUI").prop("docExpansion")).toBe("list");
    });
    
    it("renders swagger with defaultModelExpandDepth equalto 1", () => {
        const wrapper = shallow(<SwaggerPage />);
        expect(wrapper.find("SwaggerUI").prop("defaultModelExpandDepth")).toBe(1);
    });
});