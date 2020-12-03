
/* eslint-disable no-undef */

const apiKey = Cypress.env("x-api-key");

describe("Swagger Page", () => {
//     describe("layout", () => {
//         beforeEach(() => {
//             cy.visit("/#/");
//         });    
//     });
//     
//     describe("url to url endpoint", () => {
//         beforeEach(() => {
//             cy.visit("/#/");
//             cy.get(".opblock").eq(0).click();
//             cy.get(".opblock").eq(0).find(".try-out__btn").click();
//         });// 

//         it("Request details are correct should be post", () => {
//             cy.get(".opblock").eq(0).find(".opblock-summary-method").should("have.text", "POST")
//             cy.get(".opblock").eq(0).find(".parameter__name.required").contains("code");
//             cy.get(".opblock").eq(0).find(".opblock-section .content-type").contains("application/json");
//         });
//     });
//     
//     describe("file endpoint", () => {
//         beforeEach(() => {
//             cy.visit("/#/");
//             cy.get(".opblock").eq(1).click();
//             cy.get(".opblock").eq(1).find(".try-out__btn").click();
//         });// 

//         it("Request details are correct should be post", () => {
//             cy.get(".opblock").eq(1).find(".opblock-summary-method").should("have.text", "POST")
//             cy.get(".opblock").eq(1).find(".parameter__name.required").contains("code");
//             cy.get(".opblock").eq(1).find(".opblock-section .content-type").contains("multipart/form-data");
//         });
//     });
//         
//     describe("base64 endpoint", () => {
//         before(() => {
//             cy.visit("/#/");
//             cy.get(".opblock").eq(2).click();
//             cy.get(".opblock").eq(2).find(".try-out__btn").click();
//         });// 

//         it("Request details are correct should be post", () => {
//             cy.get(".opblock").eq(2).find(".opblock-summary-method").should("have.text", "POST")
//             cy.get(".opblock").eq(2).find(".parameter__name.required").contains("code");
//             cy.get(".opblock").eq(2).find(".opblock-section .content-type").contains("application/json");
//         });
//     });
});