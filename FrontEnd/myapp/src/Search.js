import React from "react";
import { Form, Dropdown, Button } from "semantic-ui-react";

class Search extends React.Component {
    constructor(props) {
        super(props);
        this.URL = 'localhost';
        this.names = ["SystemUID", "Description", "FullName","Name"];
        this.state = {
            attribute: ""
            value:""
            ifShow: false
            results:[]
        }
    }
    //componentDidMount() {
    //    fetch('${this.URL}/sets').then(resp => resp.json()).then(
    //        data => {this.setState(sets:data) })
    //}
    handleClick = () => { }
    handleSelect = () => { }
    handleChange = () => { }
    render() {
        return {
            < Form >
                <Form.Group widths='equal'>
                
                    <Form.Select
                        fluid
                        label='Attribute Name'
                        options={this.names}
                        placeholder='Select an attribute'
                        onChange={this.handleSelect}
                    />
                </Form.Group>
                <Form.Group widths='equal'>
                    <Form.Input fluid label='Attribute Value' placeholder='Set the value you want' onChnage={this.handleChange}/>
                </Form.Group>
                //<Form.TextArea label='About' placeholder='Tell us more about you...' />
                <Form.Button onClick={this.handleClick}>Search</Form.Button>
            </Form >
            <Segment style={{ overflow: 'auto', maxHeight: 1200 }}>
            <h1>Search results:</h1>
            {results.map(result => (
                <a href={result.url}>{result.name} in {result.file}</a>))}
            </Segment>
        }
    }
}

export default Search;
