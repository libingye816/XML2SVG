import React from "react";
import { Form, Dropdown, Button,Container,Segment } from "semantic-ui-react";

class Search extends React.Component {
    constructor(props) {
        super(props);
        this.URL = 'http://localhost:8080';
        this.names = [{ key: "SystemUID", value: "SystemUID", text: "SystemUID" },
            { key: "Description", value: "Description", text: "Description" },
            { key: "FullName", value: "FullName", text: "FullName" },
            { key: "Name", value: "Name", text: "Name" },
            { key: "Text String", value: "Text String", text: "Text String" }];
        //this.attribute="";
        //this.condition="";
        this.state = {
            ifShow: false,
            results: [],
            loading: false,
            attribute: "",
            condition:""
        }
    }
    //componentDidMount() {
    //    fetch(`${this.URL}/sets`).then(resp => resp.json()).then(
    //        data => {this.setState(sets:data) })
    //}
    handleClick = (e) => {
        this.setState({ loading: true });
        fetch(`${this.URL}/search/${this.state.attribute}/${this.state.condition}`).then(resp => resp.json()).then(data => { this.setState({ results: data })}).then(
            () => {
                this.setState({ loading: false });
                this.setState({ ifShow: true });
            })
    }
    handleSelect = (e, { value }) => { this.setState({ attribute : value });}
    handleChange = (e, { value }) => { this.setState({ condition: value });}
    reset = (e) => {
        this.setState({ ifShow: false,attribute:"",condition:"" });
    }
    render() {
        let resultShow = null;
        let { attribute, condition } = this.state;
        if (this.state.results.length > 0 && this.state.ifShow) {
            resultShow = (
                <Segment style={{ overflow: 'auto', maxHeight: 1200 }}>
                <h1>Search results:</h1>
                    {this.state.results.map(result => (
                        <div>
                            <a href={result.url} onClick={this.reset}>{result.Name} in {result.svgfile} (ID: {result.ID})</a>
                            <br />
                        </div>
                    )
                    )}
                </Segment>
        )
    }

        else if (this.state.results.length === 0 && this.state.ifShow) {
            
            resultShow = (<Segment style={{ overflow: 'auto', maxHeight: 1200 }}>
                <h1>Search results:</h1>
                < p > Find nothing!</p >\
                </Segment>)
        }
        return (
            <Container>
                <Form>
                    <Form.Group widths='equal'>

                        <Form.Select
                            fluid
                            label='Attribute Name'
                            options={this.names}
                            value={attribute}
                            placeholder='Select an attribute'
                            onChange={this.handleSelect}
                        />
                    </Form.Group>
                    <Form.Group widths='equal'>
                        <Form.Input fluid label='Attribute Value' value={condition} placeholder='Set the value you want' onChange={this.handleChange} />
                    </Form.Group>
                    <Form.Button onClick={this.handleClick} loading={this.state.loading}>Search</Form.Button>
                </Form>
                {resultShow}
            </Container>
        )
    }
}

export default Search;
