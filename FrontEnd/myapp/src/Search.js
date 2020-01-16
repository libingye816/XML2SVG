import React from "react";
import { Form, Dropdown, Button,Container,Segment } from "semantic-ui-react";

class Search extends React.Component {
    constructor(props) {
        super(props);
        this.URL = 'localhost:8080';
        this.names = [{ key: "SystemUID", value: "SystemUID", text: "SystemUID" },
            { key: "Description", value: "Description", text: "Description" },
            { key: "FullName", value: "FullName", text: "FullName" },
            {key: "Name", value: "Name", text: "Name" }];
        this.attribute="";
        this.condition="";
        this.state = {
            ifShow: false,
            results: [],
            loading:false
        }
    }
    //componentDidMount() {
    //    fetch(`${this.URL}/sets`).then(resp => resp.json()).then(
    //        data => {this.setState(sets:data) })
    //}
    handleClick = (e) => {
        this.setState({ loading: true });
        fetch(`${this.URL}/search/${this.attribute}/${this.condition}`).then(resp => resp.json()).then(data => { this.setState({ results: data })})
        this.setState({ loading: false });
        this.setState({ ifShow: true });
    }
    handleSelect = (e, { value }) => { this.attribute = value;}
    handleChange = (e, { value }) => { this.condition = value;}
    render() {
        let resultShow=null;

        if (this.state.results.length > 0) {
            resultShow=(
            
                this.state.results.map(result => (
                    <a href={result.url}>{result.name} in {result.file} (ID: {result.ID})</a>))
            )
        }

        else if (this.state.results.length === 0 && this.state.ifShow) {
            resultShow=( < p > Find nothing!</p >)
        }
        return (
            <Container>
                <Form>
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
                        <Form.Input fluid label='Attribute Value' placeholder='Set the value you want' onChange={this.handleChange} />
                    </Form.Group>
                    <Form.Button onClick={this.handleClick} loading={this.state.loading}>Search</Form.Button>
                </Form>
                <Segment style={{ overflow: 'auto', maxHeight: 1200 }}>
                    {this.state.ifShow ? <h1>Search results:</h1> : null}
                    {resultShow}
                </Segment>
            </Container>
        )
    }
}

export default Search;
